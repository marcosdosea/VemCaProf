using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Service;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers
{
    public class PenalidadeController : Controller
    {
        private readonly IPenalidadeService _penalidadeService;
        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;
        private readonly ILogger<PenalidadeController> _logger;


        public PenalidadeController(IPenalidadeService penalidadeService, IPessoaService pessoaService, IMapper mapper, ILogger<PenalidadeController> logger)
        {
            _penalidadeService = penalidadeService;
            _pessoaService = pessoaService;
            _mapper = mapper;
            _logger = logger;

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
            PenalidadeModel penalidadeModel = _mapper.Map<PenalidadeModel>(penalidade);
            return View(penalidadeModel);

        }

        // GET: Penalidade/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Penalidade/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PenalidadeModel penalidadeM)
        {
            if (!ModelState.IsValid)
            {
              
                return View(penalidadeM);
            }
            try
            {
                var penalidade = _mapper.Map<PenalidadeDTO>(penalidadeM);
                _penalidadeService.Create(penalidade);

                TempData["SuccessMessage"] = "Penalidade cadastrada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Professores = new SelectList(_pessoaService.GetAllProfessores(), "Id", "Nome");
                ViewBag.Professores = new SelectList(_pessoaService.GetAllResponsaveis(), "Id", "Nome");
                return View(penalidadeM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar penalidade");
                TempData["ErrorMessage"] = "Erro ao criar penalidade";
                return View(penalidadeM);
            }
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
