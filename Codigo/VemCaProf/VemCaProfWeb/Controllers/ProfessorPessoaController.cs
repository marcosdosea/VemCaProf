using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers;

public class ProfessorPessoaController : Controller
{
    IPessoaService _pessoaService;
    ICidadeService _cidadeService;
    IMapper _mapper;


    public ProfessorPessoaController(IPessoaService pessoaService, ICidadeService cidadeService, IMapper mapper)
    {
        _pessoaService = pessoaService;
        _cidadeService = cidadeService;
        _mapper = mapper;
    }

    // GET: Professor
    public ActionResult Index()
    {
        var listaEntities = _pessoaService.GetAllProfessores();
        var listaModels = _mapper.Map<List<ProfessorPessoaModel>>(listaEntities);
        return View(listaModels);
    }

    // GET: Professor/Details/5
    public ActionResult Details(int id)
    {
        var entity = _pessoaService.GetProfessor(id);
        var professorModel = _mapper.Map<ProfessorPessoaModel>(entity);
        return View(professorModel);
    }

    // GET: Professor/Create
    public ActionResult Create()
    {
        CarregarCidades();
        return View();
    }

    // POST: Professor/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ProfessorPessoaModel professorModel, IFormFile? arquivoDiploma, IFormFile? arquivoFoto)
    {
        if (ModelState.IsValid)
        {
            var dto = _mapper.Map<ProfessorPessoaDTO>(professorModel);

            // Converte arquivos para bytes
            dto.Diploma = ConvertToBytes(arquivoDiploma);
            dto.FotoPerfil = ConvertToBytes(arquivoFoto);

            // Como é Create, a senha é obrigatória (validada no Model ou aqui manualmente)
            if (string.IsNullOrEmpty(professorModel.Senha))
            {
                ModelState.AddModelError("Senha", "Senha é obrigatória");
                CarregarCidades();
                return View(professorModel);
            }

            dto.Senha = professorModel.Senha;

            _pessoaService.CreateProfessor(dto);
            return RedirectToAction(nameof(Index));
        }

        CarregarCidades();
        return View(professorModel);
    }

    // GET: Professor/Edit/5
    public ActionResult Edit(int id)
    {
        var entity = _pessoaService.GetProfessor(id);
        var professorModel = _mapper.Map<ProfessorPessoaModel>(entity);
        CarregarCidades();
        return View(professorModel);
    }

    // POST: Professor/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, ProfessorPessoaModel professorModel, IFormFile? arquivoDiploma,
        IFormFile? arquivoFoto)
    {
        if (id != professorModel.Id) return NotFound();

        ModelState.Remove("Senha");

        if (ModelState.IsValid)
        {
            var dto = _mapper.Map<ProfessorPessoaDTO>(professorModel);

            if (arquivoDiploma != null) dto.Diploma = ConvertToBytes(arquivoDiploma);
            if (arquivoFoto != null) dto.FotoPerfil = ConvertToBytes(arquivoFoto);

            _pessoaService.EditProfessor(dto);
            return RedirectToAction(nameof(Index));
        }
        CarregarCidades();
        return View(professorModel);
    }

    // DELETE
    public ActionResult Delete(int id)
    {
        var entity = _pessoaService.GetProfessor(id);
        var professorModel = _mapper.Map<ProfessorPessoaModel>(entity);
        return View(professorModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        _pessoaService.Delete(id);
        return RedirectToAction(nameof(Index));
    }
    
    private void CarregarCidades()
    {
        // Busca todas as cidades do banco
        var listaCidades = _cidadeService.GetAll(); 
        
        // Cria o SelectList que a View espera
        // "Id" é o valor que vai pro banco (value)
        // "Nome" é o texto que aparece pro usuário (text)
        ViewBag.ListaDeCidades = new SelectList(listaCidades, "Id", "Nome");
    }

    // Auxiliar para Arquivos
    private byte[]? ConvertToBytes(IFormFile? file)
    {
        if (file == null) return null;

        // DEFININDO O LIMITE (60KB em bytes)
        const long maxSizeBytes = 60000; 

        if (file.Length > maxSizeBytes)
        {
            throw new InvalidOperationException($"O arquivo '{file.FileName}' excede o limite de 60KB.");
        }

        using (var ms = new MemoryStream())
        {
            file.CopyTo(ms);
            return ms.ToArray();
        }
    }
}