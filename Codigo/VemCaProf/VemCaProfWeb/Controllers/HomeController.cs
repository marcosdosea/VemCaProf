using System.Diagnostics;
using Core.Enums;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPessoaService _pessoaService;
    private readonly IAulaService _aulaService;

    public HomeController(
        ILogger<HomeController> logger,
        IPessoaService pessoaService,
        IAulaService aulaService)
    {
        _logger = logger;
        _pessoaService = pessoaService;
        _aulaService = aulaService;
    }

    public IActionResult Index()
    {
        var hoje = DateTime.Today;
        var inicioSemana = hoje.AddDays(-(int)hoje.DayOfWeek + (int)DayOfWeek.Monday);
        var fimSemana = inicioSemana.AddDays(7);

        var todasAulas = _aulaService.GetAll().ToList();
        var todasPessoas = _pessoaService.GetAll().ToList();

        var model = new HomeViewModel
        {
            AlunosAtivos = todasPessoas.Count(p => p.TipoPessoa == "A"),
            AulasSemana = todasAulas.Count(a =>
                a.DataHorarioInicio >= inicioSemana &&
                a.DataHorarioInicio < fimSemana &&
                a.Status != StatusEnum.Cancelada),
            AulasHoje = todasAulas.Count(a =>
                a.DataHorarioInicio.Date == hoje &&
                a.Status != StatusEnum.Cancelada),
            ReceitaMes = todasAulas
                .Where(a => a.Status == StatusEnum.Paga &&
                            a.DataHoraPagamento.HasValue &&
                            a.DataHoraPagamento.Value.Year == hoje.Year &&
                            a.DataHoraPagamento.Value.Month == hoje.Month)
                .Sum(a => (decimal)a.Valor),
            NomeUsuario = _pessoaService.GetByCpf(User.Identity?.Name) is { } p
                ? $"{p.Nome} {p.Sobrenome}"
                : (User.Identity?.Name ?? "Usuário")
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
