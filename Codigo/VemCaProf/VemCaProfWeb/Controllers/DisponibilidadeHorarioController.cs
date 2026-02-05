using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Service;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers;

public class DisponibilidadeHorarioController : Controller
{
    private readonly IDisponibilidadeHorarioService _disponibilidadeHorarioService;
    private readonly IMapper _mapper;
    private readonly ILogger<DisponibilidadeHorarioController> _logger;

    public DisponibilidadeHorarioController(
        IDisponibilidadeHorarioService disponibilidadeHorarioService,
        IMapper mapper,
        ILogger<DisponibilidadeHorarioController> logger)
    {
        _disponibilidadeHorarioService = disponibilidadeHorarioService;
        _mapper = mapper;
        _logger = logger;
    }


    public IActionResult Index()
    {
        try
        {
            var disponibilidadeHorarioDto = _disponibilidadeHorarioService.GetAll();
            var disponibilidadeHorarioModel = _mapper.Map<IEnumerable<DisponibilidadeHorarioModel>>(disponibilidadeHorarioDto);
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar lista");
            TempData["ErrorMessage"] = "Erro ao carregar lista";
            return RedirectToAction("Index", "Home");
        }
    }

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
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar detalhes da ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar ";
            return RedirectToAction(nameof(Index));
        }
    }


    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]

    public IActionResult Create([Bind("Dia,HorarioInicio,HorarioFim,IdProfessor")] DisponibilidadeHorarioModel disponibilidadeHorarioModel)
    {
        if (!ModelState.IsValid)
        {
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
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar ");
            TempData["ErrorMessage"] = "Erro ao criar ";
            return View(disponibilidadeHorarioModel);
        }
    }

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
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar disponibilidade horário para edição ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar disponibilidade horário para edição";
            return RedirectToAction(nameof(Index));
        }
    }


    [HttpPost]
    public IActionResult Edit(int id, [Bind("Id,Dia,HorarioInicio,HorarioFim,IdProfessor")] DisponibilidadeHorarioModel disponibilidadeHorarioModel)
    {
        if (id != disponibilidadeHorarioModel.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
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
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao atualizar ";
            return View(disponibilidadeHorarioModel);
        }
    }

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
            return View(disponibilidadeHorarioModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar exclusão ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar exclusão";
            return RedirectToAction(nameof(Index));
        }
    }


    [HttpPost, ActionName("Delete")]

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
}