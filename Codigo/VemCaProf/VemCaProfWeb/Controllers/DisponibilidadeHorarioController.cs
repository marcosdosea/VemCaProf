using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers;

public class DisponibilidadeHorarioController : Controller
{
    private readonly IDisponibilidadeHorarioService _disponibilidadeHorarioService;
    private readonly IPessoaService _pessoaService;
    private readonly IMapper _mapper;
    private readonly ILogger<DisponibilidadeHorarioController> _logger;

    public DisponibilidadeHorarioController(
        IDisponibilidadeHorarioService disponibilidadeHorarioService,
        IPessoaService pessoaService,
        IMapper mapper,
        ILogger<DisponibilidadeHorarioController> logger)
    {
        _disponibilidadeHorarioService = disponibilidadeHorarioService;
        _pessoaService = pessoaService;
        _mapper = mapper;
        _logger = logger;
    }

    //GET : DisponibilidadeHorario
    public IActionResult Index()
    {
        try
        {
            var disponibilidadeHorarioDto = _disponibilidadeHorarioService.GetAll();
            var disponibilidadeHorarioModel = _mapper.Map<IEnumerable<DisponibilidadeHorarioModel>>(disponibilidadeHorarioDto).ToList();
            PreencherNomesProfessores(disponibilidadeHorarioModel);
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar lista");
            TempData["ErrorMessage"] = "Erro ao carregar lista";
            return RedirectToAction("Index", "Home");
        }
    }

    // GET: DisponibilidadeHorario/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var disponibilidadeHorarioDto = _disponibilidadeHorarioService.Get(id.Value);
            if (disponibilidadeHorarioDto == null)
            {
                return NotFound();
            }

            var disponibilidadeHorarioModel = _mapper.Map<DisponibilidadeHorarioModel>(disponibilidadeHorarioDto);
            PreencherNomesProfessores(new[] { disponibilidadeHorarioModel });
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar detalhes da ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar ";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: DisponibilidadeHorario/Create
    public IActionResult Create()
    {
        CarregarProfessores();
        return View();
    }

    // POST: DisponibilidadeHorario/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("HorarioInicio,HorarioFim,IdProfessor")] DisponibilidadeHorarioModel disponibilidadeHorarioModel)
    {
        if (!ModelState.IsValid)
        {
            CarregarProfessores(disponibilidadeHorarioModel.IdProfessor);
            return View(disponibilidadeHorarioModel);
        }

        try
        {
            var disponibilidadeHorarioDto = _mapper.Map<DisponibilidadeHorarioDTO>(disponibilidadeHorarioModel);
            _disponibilidadeHorarioService.Create(disponibilidadeHorarioDto);

            TempData["SuccessMessage"] = "Cadastro com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            CarregarProfessores(disponibilidadeHorarioModel.IdProfessor);
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar ");
            TempData["ErrorMessage"] = "Erro ao criar ";
            CarregarProfessores(disponibilidadeHorarioModel.IdProfessor);
            return View(disponibilidadeHorarioModel);
        }
    }
    // GET: DisponibilidadeHorario/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var disponibilidadeHorarioDto = _disponibilidadeHorarioService.Get(id.Value);
            if (disponibilidadeHorarioDto == null)
            {
                return NotFound();
            }

            var disponibilidadeHorarioModel = _mapper.Map<DisponibilidadeHorarioModel>(disponibilidadeHorarioDto);
            CarregarProfessores(disponibilidadeHorarioModel.IdProfessor);
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar disponibilidade horário para edição ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar disponibilidade horário para edição";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: DisponibilidadeHorario/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,HorarioInicio,HorarioFim,IdProfessor")] DisponibilidadeHorarioModel disponibilidadeHorarioModel)
    {
        if (id != disponibilidadeHorarioModel.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            CarregarProfessores(disponibilidadeHorarioModel.IdProfessor);
            return View(disponibilidadeHorarioModel);
        }

        try
        {
            var disponibilidadeHorarioDto = _mapper.Map<DisponibilidadeHorarioDTO>(disponibilidadeHorarioModel);
            var success = _disponibilidadeHorarioService.Update(disponibilidadeHorarioDto);

            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Horário atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            CarregarProfessores(disponibilidadeHorarioModel.IdProfessor);
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao atualizar ";
            CarregarProfessores(disponibilidadeHorarioModel.IdProfessor);
            return View(disponibilidadeHorarioModel);
        }
    }
    // GET: DisponibilidadeHorario/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var disponibilidadeHorarioDto = _disponibilidadeHorarioService.Get(id.Value);
            if (disponibilidadeHorarioDto == null)
            {
                return NotFound();
            }

            var disponibilidadeHorarioModel = _mapper.Map<DisponibilidadeHorarioModel>(disponibilidadeHorarioDto);
            PreencherNomesProfessores(new[] { disponibilidadeHorarioModel });
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar exclusão ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar exclusão";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: DisponibilidadeHorario/Delete/5

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            var success = _disponibilidadeHorarioService.Delete(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Horário não encontrada";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Horário excluída com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir Horario ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao excluir Horario";
            return RedirectToAction(nameof(Index));
        }
    }

    private void CarregarProfessores(int? professorSelecionado = null)
    {
        var professores = _pessoaService.GetAllProfessores()
            .Select(p => new
            {
                p.Id,
                Texto = $"{p.Nome} {p.Sobrenome} - {p.Email}"
            });

        ViewBag.Professores = new SelectList(professores, "Id", "Texto", professorSelecionado);
    }

    private void PreencherNomesProfessores(IEnumerable<DisponibilidadeHorarioModel> disponibilidades)
    {
        var professores = _pessoaService.GetAllProfessores()
            .ToDictionary(p => p.Id, p => $"{p.Nome} {p.Sobrenome}");

        foreach (var disponibilidade in disponibilidades)
        {
            professores.TryGetValue(disponibilidade.IdProfessor, out var professor);
            disponibilidade.NomeProfessor = professor;
        }
    }
}
