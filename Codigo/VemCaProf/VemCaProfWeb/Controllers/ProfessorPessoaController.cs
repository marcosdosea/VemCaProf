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

[Authorize(Roles = "Admin,Professor")]
public class ProfessorPessoaController : Controller
{
    IPessoaService _pessoaService;
    ICidadeService _cidadeService;
    IMapper _mapper;
    UserManager<Usuario> _userManager;

    public ProfessorPessoaController(IPessoaService pessoaService, ICidadeService cidadeService, IMapper mapper, UserManager<Usuario> userManager)
    {
        _pessoaService = pessoaService;
        _cidadeService = cidadeService;
        _mapper = mapper;
        _userManager = userManager;
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
    [Authorize]
    public ActionResult Create(string IdUsuario, string email)
    {
        if (string.IsNullOrEmpty(IdUsuario)) return RedirectToAction("Index", "Home");

        var model = new ProfessorPessoaModel 
        { 
            IdUsuario = IdUsuario, 
            Email = email 
        };
        CarregarCidades();
        return View(model);
    }

    // POST: Professor/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(ProfessorPessoaModel professorModel, IFormFile? arquivoDiploma, IFormFile? arquivoFoto)
    {
        // Validação Manual de Tamanho
        if (arquivoDiploma != null && arquivoDiploma.Length > 60000)
            ModelState.AddModelError("arquivoDiploma", "O arquivo 'diploma' excede o limite de 60KB.");
    
        if (arquivoFoto != null && arquivoFoto.Length > 60000)
            ModelState.AddModelError("arquivoFoto", "O arquivo 'foto' excede o limite de 60KB.");

        if (ModelState.IsValid)
        {
                // 3. Mapear para o DTO e converter arquivos
                var dto = _mapper.Map<ProfessorPessoaDTO>(professorModel);
                
                dto.Diploma = ConvertToBytes(arquivoDiploma);
                dto.FotoPerfil = ConvertToBytes(arquivoFoto);

                // 4. Salvar no banco de negócio SEM o ID do Identity
                // Isso respeita sua tabela original
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
        var listaCidades = _cidadeService.GetAll(); 
        ViewBag.ListaDeCidades = new SelectList(listaCidades, "Id", "Nome");
    }

    private byte[]? ConvertToBytes(IFormFile? file)
    {
        if (file == null) return null;
        const long maxSizeBytes = 60000; 

        if (file.Length > maxSizeBytes)
        {
            return null; // Ou trate com erro no ModelState
        }

        using (var ms = new MemoryStream())
        {
            file.CopyTo(ms);
            return ms.ToArray();
        }
    }
}