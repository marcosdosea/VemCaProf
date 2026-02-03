using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VemCaProfWeb.Areas.Identity.Data;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers;

[Authorize(Roles = "Admin,Aluno")] 
public class AlunoPessoaController : Controller
{
    IPessoaService _pessoaService;
    IMapper _mapper;
    UserManager<Usuario> _userManager;

    public AlunoPessoaController(IPessoaService pessoaService, IMapper mapper, UserManager<Usuario> userManager)
    {
        _pessoaService = pessoaService;
        _mapper = mapper;
        _userManager = userManager;
    }

    public ActionResult Index()
    {
        var listaEntities = _pessoaService.GetAllAlunos();
        var listaModels = _mapper.Map<List<AlunoPessoaModel>>(listaEntities);
        return View(listaModels);
    }

    public ActionResult Details(int id)
    {
        var entity = _pessoaService.GetAluno(id);
        var alunoModel = _mapper.Map<AlunoPessoaModel>(entity);
        return View(alunoModel);
    }
    [Authorize] 
    public ActionResult Create(string IdUsuario, string email)
    {
        if (string.IsNullOrEmpty(IdUsuario)) return RedirectToAction("Index", "Home");
        var model = new AlunoPessoaModel
        {
            IdUsuario =  IdUsuario,
            Email = email
        };
        CarregarResponsaveis();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(AlunoPessoaModel alunoModel)
    {
        if (ModelState.IsValid)
        {
                // 2. Mapeamos para o DTO
                var dto = _mapper.Map<AlunoPessoaDTO>(alunoModel);
                
                _pessoaService.CreateAluno(dto);

                return RedirectToAction(nameof(Index));
        }

        CarregarResponsaveis();
        return View(alunoModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, AlunoPessoaModel alunoModel)
    {
        if (id != alunoModel.Id) return NotFound();
        
        if (ModelState.IsValid)
        {
            var dto = _mapper.Map<AlunoPessoaDTO>(alunoModel);
            _pessoaService.EditAluno(dto);
            return RedirectToAction(nameof(Index));
        }

        CarregarResponsaveis();
        return View(alunoModel);
    }

    public ActionResult Delete(int id)
    {
        var entity = _pessoaService.GetAluno(id);
        var alunoModel = _mapper.Map<AlunoPessoaModel>(entity);
        return View(alunoModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        _pessoaService.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    private void CarregarResponsaveis()
    {
        var lista = _pessoaService.GetAllResponsaveis();
        var selectList = lista.Select(p => new
        {
            Id = p.Id,
            NomeExibicao = $"{p.Nome} {p.Sobrenome} (CPF: {p.Cpf})"
        });
        ViewBag.ListaResponsaveis = new SelectList(selectList, "Id", "NomeExibicao");
    }
}