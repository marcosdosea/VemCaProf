using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VemCaProfWeb.Areas.Identity.Data;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers;

[Authorize(Roles = "Admin, Responsavel")]
public class ResponsavelPessoaController : Controller
{
    private readonly IPessoaService _pessoaService;
    private readonly IMapper _mapper;
    private readonly UserManager<Usuario> _userManager;
    
    public ResponsavelPessoaController(IPessoaService pessoaService, IMapper mapper, UserManager<Usuario> userManager)
    {
        _pessoaService = pessoaService;
        _mapper = mapper;
        _userManager = userManager;
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
    [Authorize]
    public ActionResult Create(string IdUsuario , string email)
    {
        if (string.IsNullOrEmpty(IdUsuario)) return RedirectToAction("Index", "Home");

        var model = new ResponsavelPessoaModel 
        { 
            IdUsuario = IdUsuario, 
            Email = email 
        };
        return View(model);
    }

    // POST: Responsavel/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(ResponsavelPessoaModel responsavelModel)
    {
        if (ModelState.IsValid)
        {

            // 4. Mapeia para o DTO
            var dto = _mapper.Map<ResponsavelPessoaDTO>(responsavelModel);

            // 5. Salva na sua tabela SEM alterar a estrutura do banco
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