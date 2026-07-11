using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers;

public class AulaController : Controller
{
    private readonly IAulaService _aulaService;
    private readonly IDisciplinaService _disciplinaService;
    private readonly IPessoaService _pessoaService;
    private readonly IMapper _mapper;
    private readonly ILogger<AulaController> _logger;

    public AulaController(
        IAulaService aulaService,
        IDisciplinaService disciplinaService,
        IPessoaService pessoaService,
        IMapper mapper,
        ILogger<AulaController> logger)
    {
        _aulaService = aulaService;
        _disciplinaService = disciplinaService;
        _pessoaService = pessoaService;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: Aula
    public IActionResult Index()
    {
        try
        {
            var aulaDto = _aulaService.GetAll();
            var aulaModel = _mapper.Map<IEnumerable<AulaModel>>(aulaDto).ToList();
            PreencherNomesRelacionados(aulaModel);
            return View(aulaModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar lista de aulas");
            TempData["ErrorMessage"] = "Erro ao carregar lista de aulas";
            return RedirectToAction("Index", "Home");
        }
    }

    // GET: Aula/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var aulaDto = _aulaService.Get(id.Value);
            if (aulaDto == null)
            {
                return NotFound();
            }

            var aulaModel = _mapper.Map<AulaModel>(aulaDto);
            PreencherNomesRelacionados(new[] { aulaModel });
            return View(aulaModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar detalhes da aula ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar detalhes da aula";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Aula/Create
    public IActionResult Create()
    {
        CarregarOpcoes();
        return View(new AulaModel { DataAula = DateTime.Today });
    }

    // POST: Aula/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("DataAula,Descricao,Valor,IdDisciplina,IdResponsavel,IdAluno,IdProfessor," +
        "IdDisponibilidadeHorario")] AulaModel aulaModel)
    {
        if (!ModelState.IsValid)
        {
            var erros = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            _logger.LogWarning("ModelState inválido ao criar aula: {Erros}", string.Join(" | ", erros));
            CarregarOpcoes(aulaModel);
            return View(aulaModel);
        }

        try
        {
            var aulaDto = _mapper.Map<AulaDTO>(aulaModel);
            _aulaService.Create(aulaDto);

            TempData["SuccessMessage"] = "Aula cadastrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            CarregarOpcoes(aulaModel);
            return View(aulaModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar aula");
            TempData["ErrorMessage"] = "Erro ao criar aula";
            CarregarOpcoes(aulaModel);
            return View(aulaModel);
        }
    }

    // GET: Aula/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var aulaDto = _aulaService.Get(id.Value);
            if (aulaDto == null)
            {
                return NotFound();
            }

            var aulaModel = _mapper.Map<AulaModel>(aulaDto);
            aulaModel.IdDisponibilidadeHorario = EncontrarHorarioAtual(aulaModel);
            CarregarOpcoes(aulaModel);
            return View(aulaModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar aula para edição ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar aula para edição";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Aula/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,DataAula,Descricao,Valor,IdDisciplina,IdResponsavel,IdAluno,IdProfessor," +
        "IdDisponibilidadeHorario")] AulaModel aulaModel)
    {
        if (id != aulaModel.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            CarregarOpcoes(aulaModel);
            return View(aulaModel);
        }

        try
        {
            var aulaDto = _mapper.Map<AulaDTO>(aulaModel);
            var success = _aulaService.Update(aulaDto);

            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Aula atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            CarregarOpcoes(aulaModel);
            return View(aulaModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar aula ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao atualizar aula";
            CarregarOpcoes(aulaModel);
            return View(aulaModel);
        }
    }

    // GET: Aula/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var aulaDto = _aulaService.Get(id.Value);
            if (aulaDto == null)
            {
                return NotFound();
            }

            var aulaModel = _mapper.Map<AulaModel>(aulaDto);
            PreencherNomesRelacionados(new[] { aulaModel });
            return View(aulaModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar aula para exclusão ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar aula para exclusão";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Aula/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            var success = _aulaService.Delete(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Aula não encontrada";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Aula excluída com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir aula ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao excluir aula";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancelar(int id)
    {
        try
        {
            _aulaService.CancelarAula(id);
            TempData["SuccessMessage"] = "Aula cancelada com sucesso!";
        }
        catch (ServiceException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
       
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Confirmar(int id)
    {
        try
        {
            _aulaService.ConfirmarAula(id);
            TempData["SuccessMessage"] = "Participação confirmada com sucesso!";
        }
        catch (ServiceException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult HorariosDisponiveis(int professorId, DateTime dataAula, int? aulaId = null)
    {
        var horarios = _aulaService.GetHorariosDisponiveis(professorId, dataAula, aulaId)
            .Select(h => new
            {
                id = h.Id,
                texto = $"{h.HorarioInicio:hh\\:mm} às {h.HorarioFim:hh\\:mm}"
            });

        return Json(horarios);
    }

    private void CarregarOpcoes(AulaModel? aula = null)
    {
        var disciplinas = _disciplinaService.GetAll()
            .Select(d => new
            {
                d.Id,
                Texto = string.IsNullOrWhiteSpace(d.Nivel) ? d.Nome : $"{d.Nome} - {d.Nivel}"
            });

        var pessoas = _pessoaService.GetAll().ToList();
        var professores = pessoas
            .Where(p => p.TipoPessoa == "P")
            .Select(p => new { p.Id, Texto = FormatarPessoa(p.Nome, p.Sobrenome, p.Email) });
        var responsaveis = pessoas
            .Where(p => p.TipoPessoa == "R")
            .Select(p => new { p.Id, Texto = FormatarPessoa(p.Nome, p.Sobrenome, p.Email) });
        var alunos = pessoas
            .Where(p => p.TipoPessoa == "A")
            .Select(p => new { p.Id, Texto = FormatarPessoa(p.Nome, p.Sobrenome, p.Email) });

        ViewBag.Disciplinas = new SelectList(disciplinas, "Id", "Texto", aula?.IdDisciplina);
        ViewBag.Professores = new SelectList(professores, "Id", "Texto", aula?.IdProfessor);
        ViewBag.Responsaveis = new SelectList(responsaveis, "Id", "Texto", aula?.IdResponsavel);
        ViewBag.Alunos = new SelectList(alunos, "Id", "Texto", aula?.IdAluno);
    }

    private static string FormatarPessoa(string nome, string sobrenome, string email)
    {
        return $"{nome} {sobrenome} - {email}";
    }

    private int EncontrarHorarioAtual(AulaModel aula)
    {
        if (aula.IdDisponibilidadeHorario > 0)
            return aula.IdDisponibilidadeHorario;

        if (aula.DataAula == null)
            return 0;

        return _aulaService.GetHorariosDisponiveis(aula.IdProfessor, aula.DataAula.Value, aula.Id)
            .FirstOrDefault(h =>
                h.Dia.Date.Add(h.HorarioInicio) == aula.DataHorarioInicio &&
                h.Dia.Date.Add(h.HorarioFim) == aula.DataHorarioFinal)
            ?.Id ?? 0;
    }

    private void PreencherNomesRelacionados(IEnumerable<AulaModel> aulas)
    {
        var disciplinas = _disciplinaService.GetAll()
            .ToDictionary(d => (int)d.Id, d => d.Nome);
        var pessoas = _pessoaService.GetAll()
            .ToDictionary(p => p.Id, p => $"{p.Nome} {p.Sobrenome}");

        foreach (var aula in aulas)
        {
            disciplinas.TryGetValue(aula.IdDisciplina, out var disciplina);
            pessoas.TryGetValue(aula.IdResponsavel, out var responsavel);
            pessoas.TryGetValue(aula.IdAluno, out var aluno);
            pessoas.TryGetValue(aula.IdProfessor, out var professor);

            aula.NomeDisciplina = disciplina;
            aula.NomeResponsavel = responsavel;
            aula.NomeAluno = aluno;
            aula.NomeProfessor = professor;
        }
    }
}
