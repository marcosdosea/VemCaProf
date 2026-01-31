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
            var aulaModel = _mapper.Map<IEnumerable<AulaModel>>(aulasDto);
            return View(aulasModel);
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

    // POST: Cidade/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Nome,Estado")] CidadeModel cidadeModel)
    {
        if (!ModelState.IsValid)
        {
            return View(cidadeModel);
        }

        try
        {
            var cidadeDto = _mapper.Map<CidadeDTO>(cidadeModel);
            _cidadeService.Create(cidadeDto);

            TempData["SuccessMessage"] = "Cidade cadastrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(cidadeModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cidade");
            TempData["ErrorMessage"] = "Erro ao criar cidade";
            return View(cidadeModel);
        }
    }

    // GET: Cidade/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var cidadeDto = _cidadeService.Get(id.Value);
            if (cidadeDto == null)
            {
                return NotFound();
            }

            var cidadeModel = _mapper.Map<CidadeModel>(cidadeDto);
            return View(cidadeModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cidade para edição ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar cidade para edição";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Cidade/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Nome,Estado")] CidadeModel cidadeModel)
    {
        if (id != cidadeModel.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(cidadeModel);
        }

        try
        {
            var cidadeDto = _mapper.Map<CidadeDTO>(cidadeModel);
            var success = _cidadeService.Update(cidadeDto);

            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Cidade atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(cidadeModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar cidade ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao atualizar cidade";
            return View(cidadeModel);
        }
    }

    // GET: Cidade/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var cidadeDto = _cidadeService.Get(id.Value);
            if (cidadeDto == null)
            {
                return NotFound();
            }

            var cidadeModel = _mapper.Map<CidadeModel>(cidadeDto);
            return View(cidadeModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cidade para exclusão ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar cidade para exclusão";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Cidade/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            var success = _cidadeService.Delete(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Cidade não encontrada";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Cidade excluída com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir cidade ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao excluir cidade";
            return RedirectToAction(nameof(Index));
        }
    }
}