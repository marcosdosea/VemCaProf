using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers;
[Authorize(Roles = "Admin")]
public class CidadeController : Controller
{
    private readonly ICidadeService _cidadeService;
    private readonly IMapper _mapper;
    private readonly ILogger<CidadeController> _logger;

    public CidadeController(
        ICidadeService cidadeService,
        IMapper mapper,
        ILogger<CidadeController> logger)
    {
        _cidadeService = cidadeService;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: Cidade
    public IActionResult Index()
    {
        try
        {
            var cidadesDto = _cidadeService.GetAll();
            var cidadesModel = _mapper.Map<IEnumerable<CidadeModel>>(cidadesDto);
            return View(cidadesModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar lista de cidades");
            TempData["ErrorMessage"] = "Erro ao carregar lista de cidades";
            return RedirectToAction("Index", "Home");
        }
    }

    // GET: Cidade/Details/5
    public IActionResult Details(int? id)
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
            _logger.LogError(ex, "Erro ao buscar detalhes da cidade ID {Id}", id);
            TempData["ErrorMessage"] = "Erro ao buscar detalhes da cidade";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Cidade/Create
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Cidade/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
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