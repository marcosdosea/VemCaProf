using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers
{
    public class PenalidadeController : Controller
    {
        private readonly IPenalidadeService _penalidadeService;
        private readonly IMapper _mapper;


        public PenalidadeController(IPenalidadeService penalidadeService, IMapper mapper)
        {
            _penalidadeService = penalidadeService;
            _mapper = mapper;

        }

        //GET: Penalidade
        public ActionResult Index()
        {
            var listaPenalidades = _penalidadeService.GetAll();
            var listaPenalidadeModel = _mapper.Map<List<PenalidadeModel>>(listaPenalidades);
            return View(listaPenalidadeModel);

        }

        //GET: PenalidadeController/Details/5
        public ActionResult Details(int id)
        {
            Penalidade penalidade = _penalidadeService.Get(id);

            if (penalidade == null)
            {
                return NotFound();
            }
            PenalidadeModel autorModel = _mapper.Map<PenalidadeModel>(penalidade);
            return View(autorModel);

        }


        // GET: PenalidadeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: PenalidadeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PenalidadeModel penalidadeM)
        {
            if (ModelState.IsValid)
            {
                var penalidade = _mapper.Map<PenalidadeDTO>(penalidadeM);
                _penalidadeService.Create(penalidade);


            }
            return RedirectToAction(nameof(Index));
        }

        // GET: PenalidadeController/Edit/5
        public ActionResult Edit(int id)
        {

            Penalidade penalidade = _penalidadeService.Get(id);
            PenalidadeModel penalidadeModel = _mapper.Map<PenalidadeModel>(penalidade);
            return View(penalidadeModel);
        }


        // GET: PenalidadeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,PenalidadeModel penalidadeModel)
        {
            if (ModelState.IsValid)
            {
                var penalidade = _mapper.Map<PenalidadeDTO>(penalidadeModel);
                _penalidadeService.Edit(penalidade);

            }

            return RedirectToAction(nameof(Index));

        }

        // GET: PenalidadeController/Delete/5
        public ActionResult Delete(int id)
        {
            Penalidade penalidade = _penalidadeService.Get(id);
            PenalidadeModel penalidadeModel = _mapper.Map<PenalidadeModel>(penalidade);
            return View(penalidadeModel);
        }

        // POST: PenalidadeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, PenalidadeModel penalidadeModel)
        {
            _penalidadeService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
