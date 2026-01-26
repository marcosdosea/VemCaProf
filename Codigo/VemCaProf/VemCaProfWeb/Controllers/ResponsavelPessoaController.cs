using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers;

public class ResponsavelPessoaController : Controller
{
    IPessoaService _pessoaService;
    IMapper _mapper;
    
    public ResponsavelPessoaController(IPessoaService pessoaService, IMapper mapper)
    {
        _pessoaService = pessoaService;
        _mapper = mapper;
    }
  // GET: Responsavel
        public ActionResult Index()
        {
            var lista = _pessoaService.GetAllResponsaveis();
            var resposavelModel = _mapper.Map<List<ResponsavelPessoaModel>>(lista);
            return View(resposavelModel);
        }

        // GET: Responsavel/Details/5
        public ActionResult Details(int id)
        {
            var entity = _pessoaService.GetResponsavel(id);
            var responsavelModel = _mapper.Map<ResponsavelPessoaModel>(entity);
            return View(responsavelModel);
        }

        // GET: Responsavel/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Responsavel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResponsavelPessoaModel responsavelModel)
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<ResponsavelPessoaDTO>(responsavelModel);
                _pessoaService.CreateResponsavel(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(responsavelModel);
        }

        // GET: Responsavel/Edit/5
        public ActionResult Edit(int id)
        {
            var entity = _pessoaService.GetResponsavel(id);
            var responsavelModel = _mapper.Map<ResponsavelPessoaModel>(entity);
            return View(responsavelModel);
        }

        // POST: Responsavel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ResponsavelPessoaModel responsavelModel)
        {
            if (id != responsavelModel.Id) return NotFound();
            
            ModelState.Remove("Senha");

            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<ResponsavelPessoaDTO>(responsavelModel);
                _pessoaService.EditResponsavel(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(responsavelModel);
        }

        // DELETE
        public ActionResult Delete(int id)
        {
            var entity = _pessoaService.GetResponsavel(id);
            var responsavelModel = _mapper.Map<ResponsavelPessoaModel>(entity);
            return View(responsavelModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
                _pessoaService.Delete(id);
                return RedirectToAction(nameof(Index));
            
        }
    }
