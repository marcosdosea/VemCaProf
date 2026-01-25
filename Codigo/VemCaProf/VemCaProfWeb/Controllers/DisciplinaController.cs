using AutoMapper;
using Core.Service;
using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers
{
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: DisciplinaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DisciplinaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DisciplinaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DisciplinaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DisciplinaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
