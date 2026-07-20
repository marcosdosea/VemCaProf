using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using VemCaProfWeb.Areas.Identity.Data;
using VemCaProfWeb.Models;
using Util;
using Microsoft.Extensions.Logging;

namespace VemCaProfWeb.Controllers
{
    [Authorize]
    public class PessoaController : Controller
    {
        private readonly IPessoaService _pessoaService;
        private readonly ICidadeService _cidadeService;
        private readonly IDisciplinaService _disciplinaService;
        private readonly IMapper _mapper;
        private readonly ILogger<PessoaController> _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PessoaController(
            IPessoaService pessoaService,
            ICidadeService cidadeService,
            IDisciplinaService disciplinaService,
            IMapper mapper,
            ILogger<PessoaController> logger,
            UserManager<Usuario> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _pessoaService = pessoaService;
            _cidadeService = cidadeService;
            _disciplinaService = disciplinaService;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Pessoa
        //[Authorize(Roles = "Admin")]
        public IActionResult Index(string? tipo)
        {
            var cpfLogado = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");
            var pessoas = _pessoaService.GetListaParaIndex(tipo, cpfLogado, isAdmin);
            var viewModel = _mapper.Map<List<PessoaModel>>(pessoas);
            ViewBag.TipoAtual = tipo;
            return View(viewModel);
        }

        // GET: Pessoa/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var cpfLogado = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");
            var pessoa = _pessoaService.GetParaDetails(id, cpfLogado, isAdmin);
            if (pessoa == null)
                return RedirectToAction("Index", "Home");

            var viewModel = _mapper.Map<PessoaModel>(pessoa);

            if (pessoa.TipoPessoa == "A" && pessoa.ResponsavelId != null)
            {
                var responsavel = _pessoaService.Get(pessoa.ResponsavelId.Value);
                if (responsavel != null)
                {
                    viewModel.NomeResponsavel = $"{responsavel.Nome} {responsavel.Sobrenome}";
                }
            }

            if (pessoa.TipoPessoa == "A" && (isAdmin || User.IsInRole("Responsavel")))
            {
                var alunoUser = await _userManager.FindByNameAsync(pessoa.Cpf);
                if (alunoUser != null)
                {
                    var claims = await _userManager.GetClaimsAsync(alunoUser);
                    var senhaClaim = claims.FirstOrDefault(c => c.Type == "SenhaGerada");
                    if (senhaClaim != null)
                        viewModel.SenhaGerada = senhaClaim.Value;
                }
            }
            
            // Preencher listas auxiliares para exibição
            if (pessoa.InverseResponsavel != null)
            {
                viewModel.NomesDependentes = pessoa.InverseResponsavel
                    .Select(d => $"{d.Nome} {d.Sobrenome}")
                    .ToList();

                viewModel.Dependentes = _mapper.Map<List<PessoaModel>>(pessoa.InverseResponsavel);
            }

            if (pessoa.TipoPessoa == "P" && pessoa.IdDisciplinas != null)
            {
                viewModel.NomesDisciplinas = pessoa.IdDisciplinas.Select(d => d.Nome).ToList();
            }

            return View(viewModel);
        }

        // GET: Pessoa/Create
        public IActionResult Create(string tipo)
        {
            var cpfLogado = User.Identity?.Name ?? string.Empty;
            var emailLogado = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var isAdmin = User.IsInRole("Admin");
            var isProfessor = User.IsInRole("Professor");
            var isResponsavel = User.IsInRole("Responsavel");
            

            var entity = _pessoaService.GetModelParaCreate(tipo, cpfLogado, emailLogado, isAdmin, isProfessor, isResponsavel);

            if (entity?.Id > 0)
                return RedirectToAction(nameof(Edit), new { id = entity.Id });

            var viewModel = _mapper.Map<PessoaModel>(entity) ?? new PessoaModel { TipoPessoa = tipo };
            CarregarDadosParaViewModel(viewModel, tipo);
            return View(viewModel);
        }

    // POST: Pessoa/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PessoaModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            CarregarDadosParaViewModel(viewModel, viewModel.TipoPessoa);
            return View(viewModel);
        }

        viewModel.Cpf = Methods.RemoveNaoNumericos(viewModel.Cpf);

        if (string.IsNullOrEmpty(viewModel.TipoPessoa))
        {
            ModelState.AddModelError("", "Tipo de pessoa inválido. Recarregue a página e tente novamente.");
            CarregarDadosParaViewModel(viewModel, viewModel.TipoPessoa);
            return View(viewModel);
        }

        var cpfLogado = User.Identity?.Name;
        var isAdmin = User.IsInRole("Admin");
        var isProfessor = User.IsInRole("Professor");
        var isResponsavel = User.IsInRole("Responsavel");

        try
        {
            var pessoa = _mapper.Map<Pessoa>(viewModel);
            pessoa.Diploma = ConvertToBytes(viewModel.ArquivoDiploma);
            pessoa.FotoPerfil = ConvertToBytes(viewModel.ArquivoFotoPerfil);
            pessoa.FotoDocumento = ConvertToBytes(viewModel.ArquivoFotoDocumento);

            _pessoaService.CreateSeguro(pessoa, cpfLogado, isAdmin, isProfessor, isResponsavel);

            if (pessoa.TipoPessoa == "A")
            {
                var userPorCpf = await _userManager.FindByNameAsync(pessoa.Cpf);
                var email = pessoa.Email ?? $"{pessoa.Cpf}@vemcaprof.com";
                var userPorEmail = await _userManager.FindByEmailAsync(email);

                if (userPorCpf == null && userPorEmail == null)
                {
                    var alunoUser = new Usuario
                    {
                        UserName = pessoa.Cpf,
                        Email = email,
                        EmailConfirmed = true
                    };

                    string senha = GerarSenha();
                    var result = await _userManager.CreateAsync(alunoUser, senha);
                    if (result.Succeeded)
                    {
                        if (!await _roleManager.RoleExistsAsync("Aluno"))
                            await _roleManager.CreateAsync(new IdentityRole("Aluno"));
                        await _userManager.AddToRoleAsync(alunoUser, "Aluno");
                        await _userManager.AddClaimAsync(alunoUser, new System.Security.Claims.Claim("SenhaGerada", senha));
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Aluno cadastrado!";
                    }
                }
                else
                {
                    TempData["SuccessMessage"] = "Aluno cadastrado!";
                }
            }
            else
            {
                TempData["SuccessMessage"] = "Pessoa cadastrada com sucesso!";
            }
            return RedirectToAction(nameof(Details), new { id = pessoa.Id });
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError("", ex.Message);
            CarregarDadosParaViewModel(viewModel, viewModel.TipoPessoa);
            return View(viewModel);
        }
    }

        // GET: Pessoa/Edit/5
        public IActionResult Edit(int id)
        {
            var cpfLogado = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");
            var pessoa = _pessoaService.GetParaEdit(id, cpfLogado, isAdmin);
            if (pessoa == null)
                return RedirectToAction("Index", "Home");

            var viewModel = _mapper.Map<PessoaModel>(pessoa);
            CarregarDadosParaViewModel(viewModel, pessoa.TipoPessoa);
            return View(viewModel);
        }

    // POST: Pessoa/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, PessoaModel viewModel)
    {
        if (id != viewModel.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            CarregarDadosParaViewModel(viewModel, viewModel.TipoPessoa);
            return View(viewModel);
        }

        viewModel.Cpf = Methods.RemoveNaoNumericos(viewModel.Cpf);
        
        var cpfLogado = User.Identity?.Name;
        var isAdmin = User.IsInRole("Admin");

        try
        {
            var pessoa = _mapper.Map<Pessoa>(viewModel);

            if (viewModel.ArquivoDiploma != null)
                pessoa.Diploma = ConvertToBytes(viewModel.ArquivoDiploma);
            if (viewModel.ArquivoFotoPerfil != null)
                pessoa.FotoPerfil = ConvertToBytes(viewModel.ArquivoFotoPerfil);
            if (viewModel.ArquivoFotoDocumento != null)
                pessoa.FotoDocumento = ConvertToBytes(viewModel.ArquivoFotoDocumento);

            bool sucesso = _pessoaService.EditSeguro(pessoa, cpfLogado, isAdmin);
            if (!sucesso)
            {
                ModelState.AddModelError("", "Não foi possível editar. Verifique as permissões.");
                CarregarDadosParaViewModel(viewModel, viewModel.TipoPessoa);
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Dados atualizados com sucesso!";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError("", ex.Message);
            CarregarDadosParaViewModel(viewModel, viewModel.TipoPessoa);
            return View(viewModel);
        }
    }

        // GET: Pessoa/Delete/5
        public IActionResult Delete(int id)
        {
            var cpfLogado = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");
            var pessoa = _pessoaService.GetParaDelete(id, cpfLogado, isAdmin);
            if (pessoa == null)
                return RedirectToAction("Index", "Home");

            var viewModel = _mapper.Map<PessoaModel>(pessoa);
            return View(viewModel);
        }

        // POST: Pessoa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var pessoa = _pessoaService.Get(id);
                var cpfPessoa = pessoa?.Cpf;

                var cpfLogado = User.Identity?.Name;
                var isAdmin = User.IsInRole("Admin");
                bool sucesso = _pessoaService.DeleteSeguro(id, cpfLogado, isAdmin);
                if (!sucesso)
                {
                    TempData["ErrorMessage"] = "Não foi possível excluir.";
                    return RedirectToAction(nameof(Index));
                }

                if (cpfPessoa != null)
                {
                    var userIdentity = await _userManager.FindByNameAsync(cpfPessoa);
                    if (userIdentity != null)
                    {
                        await _userManager.DeleteAsync(userIdentity);
                    }
                }

                TempData["SuccessMessage"] = "Pessoa excluída com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao excluir pessoa";
                _logger.LogError(ex, "Erro ao excluir pessoa ID {Id}", id);
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult MeuPerfil()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Index));

            var cpfLogado = User.Identity?.Name;
            var pessoa = _pessoaService.GetByCpf(cpfLogado);
            if (pessoa == null)
            {
                // Se não tiver perfil, redireciona para criar baseado na role
                string tipo = User.IsInRole("Professor") ? "P" :
                    User.IsInRole("Aluno") ? "A" :
                    User.IsInRole("Responsavel") ? "R" : "";
                return RedirectToAction("Create", new { tipo });
            }
            return RedirectToAction("Details", new { id = pessoa.Id });
        }
        
        /// <summary>
        /// Metodos Auxiliares para carregar dados de dropdowns e converter arquivos para bytes
        /// </summary>

        private void CarregarDadosParaViewModel(PessoaModel viewModel, string? tipo)
        {
            if (tipo == null) return;
            
            if (tipo == "P" || tipo == "R")
            {
                var cidades = _cidadeService.GetAll()
                    .Select(c => new { c.Id, Texto = $"{c.Nome}/{c.Estado}" });
                viewModel.Cidades = new SelectList(cidades, "Id", "Texto", viewModel.IdCidade);
            }

            if (tipo == "P")
            {
                viewModel.Disciplinas = new SelectList(_disciplinaService.GetAll(), "Id", "Nome", viewModel.IdDisciplinas);
            }

            if (tipo == "A" && User.IsInRole("Admin"))
            {
                var responsaveis = _pessoaService.GetAllResponsaveis();
                var opcoes = responsaveis.Select(p => new
                {
                    p.Id,
                    Texto = $"{p.Nome} {p.Sobrenome} - {p.Email}"
                });
                viewModel.Responsaveis = new SelectList(opcoes, "Id", "Texto", viewModel.ResponsavelId);
            }
        }

        private static string GerarSenha()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray()) + "Aa1";
        }

        private byte[]? ConvertToBytes(IFormFile? file)
        {
            if (file == null) return null;
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
