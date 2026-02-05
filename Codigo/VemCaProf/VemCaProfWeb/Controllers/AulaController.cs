using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Service;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers;

public class AulaController : Controller
{
    private readonly IAulaService _aulaService;
    private readonly IMapper _mapper;
    private readonly ILogger<AulaController> _logger;

    public AulaController(
        IAulaService aulaService,
        IMapper mapper,
        ILogger<AulaController> logger)
    {
        _aulaService = aulaService;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: Aula
    public IActionResult Index()
    {
        try
        {
            var aulaDto = _aulaService.GetAll();
            var aulaModel = _mapper.Map<IEnumerable<AulaModel>>(aulaDto);
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
        return View();
    }

    // POST: Aula/Create
    [HttpPost]

    public IActionResult Create([Bind("DataHorarioInicio,DataHorarioFinal,Descricao,Status,Valor,MetodoPagamento,IdDisciplina" +
        ",IdResponsavel,IdAluno,IdProfessor")] AulaModel aulaModel)
    {


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
            return View(aulaModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar aula");
            TempData["ErrorMessage"] = "Erro ao criar aula";
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

    public IActionResult Edit(int id, [Bind("Id,DataHorarioInicio,DataHorarioFinal,Descricao,Status,Valor," +
        "MetodoPagamento,IdDisciplina,IdResponsavel,IdAluno,IdProfessor")] AulaModel aulaModel)
    {
        if (id != aulaModel.Id)
        {
            return NotFound();
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
            return View(aulaModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar aula ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao atualizar aula";
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
        _aulaService.CancelarAula(id);

        TempData["SuccessMessage"] = "Aula cancelada com sucesso!";
        return RedirectToAction(nameof(Index));
    }
}