using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using VemCaProfWeb.Models;
using Util;

namespace VemCaProfWeb.Controllers
{
    [Authorize]
    public class PessoaController : Controller
    {
        private readonly IPessoaService _pessoaService;
        private readonly ICidadeService _cidadeService;
        private readonly IDisciplinaService _disciplinaService;
        private readonly IMapper _mapper;

        public PessoaController(
            IPessoaService pessoaService,
            ICidadeService cidadeService,
            IDisciplinaService disciplinaService,
            IMapper mapper)
        {
            _pessoaService = pessoaService;
            _cidadeService = cidadeService;
            _disciplinaService = disciplinaService;
            _mapper = mapper;
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
        public IActionResult Details(int id)
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
    public IActionResult Create(PessoaModel viewModel)
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

            TempData["Success"] = "Pessoa cadastrada com sucesso!";
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

            TempData["Success"] = "Dados atualizados com sucesso!";
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
        public IActionResult DeleteConfirmed(int id)
        {
            var cpfLogado = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");
            bool sucesso = _pessoaService.DeleteSeguro(id, cpfLogado, isAdmin);
            if (!sucesso)
            {
                TempData["Error"] = "Não foi possível excluir.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Pessoa excluída com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult MeuPerfil()
        {
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

        private byte[]? ConvertToBytes(IFormFile? file)
        {
            if (file == null) return null;
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
