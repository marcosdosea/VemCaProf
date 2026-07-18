(function () {
  "use strict";

  var wrapper = document.querySelector(".admin-wrapper");
  var sidebar = document.querySelector(".admin-sidebar");
  var openButton = document.querySelector("[data-admin-menu-open]");
  var closeButton = document.querySelector("[data-admin-menu-close]");
  var mobileQuery = window.matchMedia("(max-width: 991.98px)");

  if (!wrapper || !sidebar || !openButton || !closeButton) {
    return;
  }

  function updateAccessibility(isOpen) {
    openButton.setAttribute("aria-expanded", String(isOpen));

    if (mobileQuery.matches) {
      sidebar.setAttribute("aria-hidden", String(!isOpen));
      sidebar.toggleAttribute("inert", !isOpen);
    } else {
      sidebar.removeAttribute("aria-hidden");
      sidebar.removeAttribute("inert");
    }
  }

  function setMenuOpen(isOpen, restoreFocus) {
    var shouldOpen = Boolean(isOpen && mobileQuery.matches);

    wrapper.classList.toggle("sidebar-open", shouldOpen);
    document.body.classList.toggle("admin-menu-open", shouldOpen);
    updateAccessibility(shouldOpen);

    if (shouldOpen) {
      window.requestAnimationFrame(function () {
        closeButton.focus();
      });
    } else if (restoreFocus) {
      openButton.focus();
    }
  }

  openButton.addEventListener("click", function () {
    setMenuOpen(true, false);
  });

  closeButton.addEventListener("click", function () {
    setMenuOpen(false, true);
  });

  sidebar.querySelectorAll(".admin-nav-item").forEach(function (link) {
    link.addEventListener("click", function () {
      setMenuOpen(false, false);
    });
  });

  document.addEventListener("keydown", function (event) {
    if (event.key === "Escape" && wrapper.classList.contains("sidebar-open")) {
      setMenuOpen(false, true);
    }
  });

  mobileQuery.addEventListener("change", function () {
    setMenuOpen(false, false);
  });

  updateAccessibility(false);
}());
