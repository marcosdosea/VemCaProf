using Core.DTO;
using Core.Enums;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers
{
    public class PagamentoController : Controller
    {
        private readonly IPagamentoService _pagamentoService;

        public PagamentoController(IPagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
        }

        public IActionResult Index()
        {
            var aulas = _pagamentoService.ListarPagamentos();
            return View(aulas);
        }

        public IActionResult Details(int id)
        {
            var aula = _pagamentoService.BuscarPorAula(id);
            if (aula == null) return NotFound();

            return View(aula);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            var aula = _pagamentoService.BuscarPorAula(id);
            if (aula == null) return NotFound();

            ViewBag.Metodos = new SelectList(new[]
            {
                new { Value = MetodoPagamentoEnum.Pix, Text = "Pix" },
                new { Value = MetodoPagamentoEnum.Credito, Text = "Crédito" },
                new { Value = MetodoPagamentoEnum.Debito, Text = "Débito" }
            }, "Value", "Text");

            var model = new PagamentoModel
            {
                IdAula = aula.Id,
                DescricaoAula = aula.Descricao,
                Valor = aula.Valor,
                MetodoPagamento = aula.MetodoPagamento
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(PagamentoModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _pagamentoService.RealizarPagamento(new RealizarPagamentoDTO
                {
                    IdAula = model.IdAula,
                    MetodoPagamento = model.MetodoPagamento
                });

                return RedirectToAction(nameof(Details), new { id = model.IdAula });
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}
