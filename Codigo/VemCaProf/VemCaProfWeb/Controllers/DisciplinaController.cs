using AutoMapper;
using Core.Service;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers
{
    [Authorize(Roles = "Admin, Professor")]
    public class DisciplinaController : Controller
    {

        IDisciplinaService _disciplinaService;
        IMapper _mapper;

        public DisciplinaController(IDisciplinaService disciplinaService, IMapper mapper)
        {
            _disciplinaService = disciplinaService;
            _mapper = mapper;
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
            if (ModelState.IsValid)
            {
                var disciplina = _mapper.Map<Disciplina>(disciplinaModel);
                _disciplinaService.Create(disciplina);
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
            if (ModelState.IsValid)
            {
                var disciplina = _mapper.Map<Disciplina>(disciplinaModel);
                _disciplinaService.Edit(disciplina);
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
            _disciplinaService.Delete((uint)id);
            return RedirectToAction(nameof(Index));
        }
    }
}
