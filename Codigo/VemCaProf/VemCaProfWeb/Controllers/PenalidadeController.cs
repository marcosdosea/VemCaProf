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
        public IActionResult Index()
        {
            var listaPenalidades = _penalidadeService.GetAll();
            var listaPenalidadeModel = _mapper.Map<IEnumerable<PenalidadeModel>>(listaPenalidades);
            
            foreach (var penalidade in listaPenalidadeModel)
            {
                var professor = _pessoaService.Get(penalidade.IdProfessor);
                var responsavel = _pessoaService.Get(penalidade.IdResponsavel);
                penalidade.NomeProfessor = professor == null ? null : $"{professor.Nome} {professor.Sobrenome}";
                penalidade.NomeResponsavel = responsavel == null ? null : $"{responsavel.Nome} {responsavel.Sobrenome}";
            }
            return View(listaPenalidadeModel);

        }

        //GET: PenalidadeController/Details/5
        public ActionResult Details(int id)
        {
            PenalidadeDTO penalidade = _penalidadeService.Get(id);

            if (penalidade == null)
            {
                return NotFound();
            }
            PenalidadeModel penalidadeModel = _mapper.Map<PenalidadeModel>(penalidade);
            var professor = _pessoaService.Get(penalidadeModel.IdProfessor);
            var responsavel = _pessoaService.Get(penalidadeModel.IdResponsavel);
            penalidadeModel.NomeProfessor = professor == null ? null : $"{professor.Nome} {professor.Sobrenome}";
            penalidadeModel.NomeResponsavel = responsavel == null ? null : $"{responsavel.Nome} {responsavel.Sobrenome}";
            return View(penalidadeModel);

        }

        // GET: Penalidade/Create
        public IActionResult Create()
        {
            RecarregarViewBags();
            return View();
        }

        // GET: Penalidade/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PenalidadeModel penalidadeM)
        {
            if (!ModelState.IsValid)
            {
                // Filtra a lista geral para pegar apenas Professores (P) e Responsáveis (R)
                RecarregarViewBags();
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
                // Filtra a lista geral para pegar apenas Professores (P) e Responsáveis (R)
                RecarregarViewBags();
                return View(penalidadeM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar penalidade");
                TempData["ErrorMessage"] = "Erro ao criar penalidade";
                RecarregarViewBags();
                return View(penalidadeM);
            }
        }

        // GET: PenalidadeController/Edit/5
        public IActionResult Edit(int id)
        {
            RecarregarViewBags();
            PenalidadeDTO penalidadeDTO = _penalidadeService.Get(id);
            if (penalidadeDTO == null) return NotFound();
            PenalidadeModel penalidadeModel = _mapper.Map<PenalidadeModel>(penalidadeDTO);                                 
            return View(penalidadeModel);
        }


        // POST: PenalidadeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PenalidadeModel penalidadeModel)
        {
            
            if (!ModelState.IsValid)
            {
                RecarregarViewBags();
                return View(penalidadeModel);
            }
            try
            {
                var penalidadeDTO = _mapper.Map<PenalidadeDTO>(penalidadeModel);
                _penalidadeService.Edit(penalidadeDTO);

                TempData["SuccessMessage"] = "Penalidade atualizada com sucesso!";
                return RedirectToAction(nameof(Index));
                
            }
             
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                RecarregarViewBags();
                return View(penalidadeModel);
            }
          }

        // GET: PenalidadeController/Delete/5
        public ActionResult Delete(int id)
        {

            PenalidadeDTO penalidade = _penalidadeService.Get(id);
            if (penalidade == null) return NotFound();
            PenalidadeModel penalidadeModel = _mapper.Map<PenalidadeModel>(penalidade);
            var professor = _pessoaService.Get(penalidadeModel.IdProfessor);
            var responsavel = _pessoaService.Get(penalidadeModel.IdResponsavel);
            penalidadeModel.NomeProfessor = professor == null ? null : $"{professor.Nome} {professor.Sobrenome}";
            penalidadeModel.NomeResponsavel = responsavel == null ? null : $"{responsavel.Nome} {responsavel.Sobrenome}";
            return View(penalidadeModel);
        }

        // POST: PenalidadeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, PenalidadeModel penalidadeModel)
        {
            try
            {
                _penalidadeService.Delete(id);
                TempData["SuccessMessage"] = "Penalidade removida com sucesso!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir penalidade");
                TempData["ErrorMessage"] = "Erro ao excluir penalidade";
            }
            return RedirectToAction(nameof(Index));
        }

        private void RecarregarViewBags()
        {
            var todasPessoas = _pessoaService.GetAll();
            var professores = todasPessoas
                .Where(p => p.TipoPessoa == "P")
                .Select(p => new { p.Id, Texto = $"{p.Nome} {p.Sobrenome} - {p.Email}" });
            var responsaveis = todasPessoas
                .Where(p => p.TipoPessoa == "R")
                .Select(p => new { p.Id, Texto = $"{p.Nome} {p.Sobrenome} - {p.Email}" });

            ViewBag.Professores = new SelectList(professores, "Id", "Texto");
            ViewBag.Responsaveis = new SelectList(responsaveis, "Id", "Texto");
        }

    }
}
