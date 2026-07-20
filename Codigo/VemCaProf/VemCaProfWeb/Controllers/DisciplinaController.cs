using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DisciplinaController : Controller
    {
        IDisciplinaService _disciplinaService;
        IMapper _mapper;
        ILogger<DisciplinaController> _logger;

        public DisciplinaController(IDisciplinaService disciplinaService, IMapper mapper, ILogger<DisciplinaController> logger)
        {
            _disciplinaService = disciplinaService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: DisciplinaController
        public ActionResult Index()
        {
            var listaDisciplinas = _disciplinaService.GetAll();
            var listaDisciplinasModel = _mapper.Map<List<DisciplinaModel>>(listaDisciplinas);
            return View(listaDisciplinasModel);
        }

        // GET: DisciplinaController/Details/5
        public ActionResult Details(int id)
        {
            Disciplina disciplina = _disciplinaService.Get((uint)id);
            DisciplinaModel disciplinaModel = _mapper.Map<DisciplinaModel>(disciplina);
            return View(disciplinaModel);
        }

        // GET: DisciplinaController/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: DisciplinaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(DisciplinaModel disciplinaModel)
        {
            if (!ModelState.IsValid)
                return View(disciplinaModel);

            try
            {
                var disciplina = _mapper.Map<Disciplina>(disciplinaModel);
                _disciplinaService.Create(disciplina);
                TempData["SuccessMessage"] = "Disciplina cadastrada com sucesso!";
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar disciplina");
                TempData["ErrorMessage"] = "Erro ao criar disciplina";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: DisciplinaController/Edit/5
        public ActionResult Edit(uint id1, int id)
        {
            Disciplina disciplina = _disciplinaService.Get((uint)id);
            DisciplinaModel disciplinaModel = _mapper.Map<DisciplinaModel>(disciplina);
            return View(disciplinaModel);
        }

        // POST: DisciplinaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, DisciplinaModel disciplinaModel)
        {
            if (!ModelState.IsValid)
                return View(disciplinaModel);

            try
            {
                var disciplina = _mapper.Map<Disciplina>(disciplinaModel);
                _disciplinaService.Edit(disciplina);
                TempData["SuccessMessage"] = "Disciplina atualizada com sucesso!";
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar disciplina");
                TempData["ErrorMessage"] = "Erro ao editar disciplina";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: DisciplinaController/Delete/5
        public ActionResult Delete(int id)
        {
            Disciplina disciplina = _disciplinaService.Get((uint)id);
            DisciplinaModel disciplinaModel = _mapper.Map<DisciplinaModel>(disciplina);
            return View(disciplinaModel);
        }

        // POST: DisciplinaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _disciplinaService.Delete((uint)id);
                TempData["SuccessMessage"] = "Disciplina excluída com sucesso!";
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir disciplina ID {Id}", id);
                TempData["ErrorMessage"] = "Erro ao excluir disciplina";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
