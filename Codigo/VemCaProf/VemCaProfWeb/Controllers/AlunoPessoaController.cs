using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers;

public class AlunoPessoaController : Controller
{
    IPessoaService _pessoaService;
    IMapper _mapper;

    public AlunoPessoaController(IPessoaService pessoaService, IMapper mapper)
    {
        _pessoaService = pessoaService;
        _mapper = mapper;
    }

    // GET : Aluno
    public ActionResult Index()
    {
        var listaEntities = _pessoaService.GetAllAlunos();
        var listaModels = _mapper.Map<List<AlunoPessoaModel>>(listaEntities);
        return View(listaModels);
    }

    // GET: Aluno/Details/5
    public ActionResult Details(int id)
    {
        var entity = _pessoaService.GetAluno(id);
        var alunoModel = _mapper.Map<AlunoPessoaModel>(entity);
        return View(alunoModel);
    }

    // GET: Aluno/Create
    public ActionResult Create()
    {
        CarregarResponsaveis();
        return View();
    }

    // POST: Aluno/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(AlunoPessoaModel alunoModel)
    {
        if (ModelState.IsValid)
        {
            var dto = _mapper.Map<AlunoPessoaDTO>(alunoModel);
            _pessoaService.CreateAluno(dto);
            return RedirectToAction(nameof(Index));
        }

        CarregarResponsaveis();
        return View(alunoModel);
    }

    // GET: Aluno/Edit/5
    public ActionResult Edit(int id)
    {
        var entity = _pessoaService.GetAluno(id);
        var alunoModel = _mapper.Map<AlunoPessoaModel>(entity);

        CarregarResponsaveis();
        return View(alunoModel);
    }

    // POST: Aluno/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, AlunoPessoaModel alunoModel)
    {
        if (id != alunoModel.Id) return NotFound();

        ModelState.Remove("Senha");

        if (ModelState.IsValid)
        {
            var dto = _mapper.Map<AlunoPessoaDTO>(alunoModel);
            _pessoaService.EditAluno(dto);
            return RedirectToAction(nameof(Index));
        }

        CarregarResponsaveis();
        return View(alunoModel);
    }

    // DELETE
    public ActionResult Delete(int id)
    {
        var entity = _pessoaService.GetAluno(id);
        var alunoModel = _mapper.Map<AlunoPessoaModel>(entity);
        return View(alunoModel);
    }

    // POST: Aluno/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        _pessoaService.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    // --- MÉTODO AUXILIAR CARREGAR RESPONSÁVEIS ---
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