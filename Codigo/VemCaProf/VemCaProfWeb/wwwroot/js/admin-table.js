(function () {
  "use strict";

  function createElement(tagName, className, text) {
    var element = document.createElement(tagName);

    if (className) {
      element.className = className;
    }

    if (text !== undefined) {
      element.textContent = text;
    }

    return element;
  }

  function removeServerEmptyRows(table) {
    var body = table.tBodies[0];

    if (!body) {
      return;
    }

    Array.from(body.rows).forEach(function (row) {
      if (row.cells.length === 1 && row.cells[0].colSpan > 1) {
        row.remove();
      }
    });
  }

  function createToolbar(table) {
    var toolbar = createElement("div", "admin-table-toolbar");
    var search = createElement("div", "admin-table-search");
    var searchIcon = createElement("i", "bi bi-search");
    var input = createElement("input", "admin-table-search-input");
    var clearButton = createElement("button", "admin-table-search-clear");
    var clearIcon = createElement("i", "bi bi-x-lg");
    var hint = createElement("div", "admin-table-search-hint");
    var hintIcon = createElement("i", "bi bi-funnel");
    var hintText = createElement("span", null, "Pesquisa em todos os campos");

    search.setAttribute("role", "search");
    searchIcon.setAttribute("aria-hidden", "true");

    input.type = "search";
    input.placeholder = table.dataset.searchPlaceholder || "Buscar registros...";
    input.setAttribute("aria-label", input.placeholder);
    input.autocomplete = "off";
    input.spellcheck = false;

    clearButton.type = "button";
    clearButton.hidden = true;
    clearButton.setAttribute("aria-label", "Limpar pesquisa");
    clearButton.title = "Limpar pesquisa";

    clearButton.appendChild(clearIcon);
    search.append(searchIcon, input, clearButton);
    hint.append(hintIcon, hintText);
    toolbar.append(search, hint);

    return {
      element: toolbar,
      input: input,
      clearButton: clearButton
    };
  }

  function createFooter() {
    var footer = createElement("div", "admin-table-footer");
    var summary = createElement("div", "admin-table-summary");
    var pagination = createElement("nav", "admin-table-pagination");

    summary.setAttribute("aria-live", "polite");
    pagination.setAttribute("aria-label", "Paginação da tabela");
    footer.append(summary, pagination);

    return {
      element: footer,
      summary: summary,
      pagination: pagination
    };
  }

  function createStrong(text) {
    return createElement("strong", null, text);
  }

  function renderSummary(summary, info, hasSearch) {
    summary.replaceChildren();

    if (info.recordsDisplay === 0) {
      summary.textContent = hasSearch
        ? "Nenhum registro encontrado para esta busca"
        : "Nenhum registro disponível";
      return;
    }

    summary.append(
      document.createTextNode("Mostrando "),
      createStrong(String(info.start + 1) + "–" + String(info.end)),
      document.createTextNode(" de "),
      createStrong(String(info.recordsDisplay)),
      document.createTextNode(info.recordsDisplay === 1 ? " registro" : " registros")
    );

    if (info.recordsDisplay !== info.recordsTotal) {
      summary.append(
        document.createTextNode(" (filtrados de "),
        createStrong(String(info.recordsTotal)),
        document.createTextNode(")")
      );
    }
  }

  function getVisiblePages(currentPage, totalPages) {
    if (totalPages <= 7) {
      return Array.from({ length: totalPages }, function (_, index) {
        return index;
      });
    }

    var pages = [0];
    var start = Math.max(1, currentPage - 1);
    var end = Math.min(totalPages - 2, currentPage + 1);

    if (currentPage <= 3) {
      end = 4;
    }

    if (currentPage >= totalPages - 4) {
      start = totalPages - 5;
    }

    if (start > 1) {
      pages.push("ellipsis");
    }

    for (var page = start; page <= end; page += 1) {
      pages.push(page);
    }

    if (end < totalPages - 2) {
      pages.push("ellipsis");
    }

    pages.push(totalPages - 1);
    return pages;
  }

  function createPaginationButton(options) {
    var button = createElement("button", "admin-table-page-button");

    button.type = "button";
    button.disabled = Boolean(options.disabled);
    button.setAttribute("aria-label", options.ariaLabel || options.label);

    if (options.active) {
      button.classList.add("active");
      button.setAttribute("aria-current", "page");
    }

    if (options.navigation) {
      button.classList.add("admin-table-page-navigation");
      var icon = createElement("i", "bi " + options.icon);
      var label = createElement("span", "admin-table-page-label", options.label);
      button.append(icon, label);
    } else {
      button.textContent = options.label;
    }

    if (!button.disabled && options.onClick) {
      button.addEventListener("click", options.onClick);
    }

    return button;
  }

  function renderPagination(pagination, api, info) {
    pagination.replaceChildren();

    pagination.appendChild(createPaginationButton({
      label: "Anterior",
      ariaLabel: "Página anterior",
      icon: "bi-chevron-left",
      navigation: true,
      disabled: info.page <= 0,
      onClick: function () {
        api.page("previous").draw("page");
      }
    }));

    getVisiblePages(info.page, info.pages).forEach(function (page) {
      if (page === "ellipsis") {
        pagination.appendChild(createElement("span", "admin-table-page-ellipsis", "…"));
        return;
      }

      pagination.appendChild(createPaginationButton({
        label: String(page + 1),
        ariaLabel: "Ir para a página " + String(page + 1),
        active: page === info.page,
        onClick: function () {
          api.page(page).draw("page");
        }
      }));
    });

    pagination.appendChild(createPaginationButton({
      label: "Próximo",
      ariaLabel: "Próxima página",
      icon: "bi-chevron-right",
      navigation: true,
      disabled: info.pages === 0 || info.page >= info.pages - 1,
      onClick: function () {
        api.page("next").draw("page");
      }
    }));
  }

  function initTable(table) {
    var $ = window.jQuery;
    var responsiveContainer = table.closest(".table-responsive");

    if (!$ || !$.fn || !$.fn.DataTable || !responsiveContainer) {
      return;
    }

    if (table.dataset.adminTableReady === "true" || $.fn.DataTable.isDataTable(table)) {
      return;
    }

    removeServerEmptyRows(table);

    var toolbar = createToolbar(table);
    var footer = createFooter();
    var actionColumn = Number.parseInt(table.dataset.actionsColumn, 10);
    var orderColumn = Number.parseInt(table.dataset.orderColumn, 10);
    var orderDirection = table.dataset.orderDirection === "desc" ? "desc" : "asc";
    var pageSize = Number.parseInt(table.dataset.pageSize, 10) || 10;
    var columnDefs = [];

    if (Number.isInteger(actionColumn)) {
      columnDefs.push({
        targets: actionColumn,
        orderable: false,
        searchable: false
      });
    }

    responsiveContainer.before(toolbar.element);
    responsiveContainer.after(footer.element);

    var options = {
      pageLength: pageSize,
      lengthChange: false,
      ordering: true,
      searching: true,
      paging: true,
      info: false,
      autoWidth: false,
      layout: {
        topStart: null,
        topEnd: null,
        bottomStart: null,
        bottomEnd: null
      },
      language: {
        emptyTable: table.dataset.emptyMessage || "Nenhum registro encontrado.",
        zeroRecords: "Nenhum registro encontrado para esta busca."
      },
      columnDefs: columnDefs
    };

    if (Number.isInteger(orderColumn)) {
      options.order = [[orderColumn, orderDirection]];
    }

    var api = $(table).DataTable(options);

    function render() {
      var info = api.page.info();
      renderSummary(footer.summary, info, toolbar.input.value.trim().length > 0);
      renderPagination(footer.pagination, api, info);
    }

    toolbar.input.addEventListener("input", function () {
      toolbar.clearButton.hidden = toolbar.input.value.length === 0;
      api.search(toolbar.input.value).draw();
    });

    toolbar.input.addEventListener("keydown", function (event) {
      if (event.key === "Escape" && toolbar.input.value) {
        toolbar.input.value = "";
        toolbar.clearButton.hidden = true;
        api.search("").draw();
      }
    });

    toolbar.clearButton.addEventListener("click", function () {
      toolbar.input.value = "";
      toolbar.clearButton.hidden = true;
      api.search("").draw();
      toolbar.input.focus();
    });

    $(table).on("draw.dt", render);
    table.dataset.adminTableReady = "true";
    render();
  }

  function initAllTables() {
    document.querySelectorAll("table[data-admin-table]").forEach(initTable);
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initAllTables);
  } else {
    initAllTables();
  }
}());
