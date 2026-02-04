using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using VemCaProfWeb.Models;

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

        // GET: PessoaController
        public ActionResult Index(string? tipo)
        {
            var listaPessoas = _pessoaService.GetAll();
            IEnumerable<Pessoa> listaEntities;

            if (!string.IsNullOrEmpty(tipo))
            {
                listaEntities = listaPessoas.Where(p => p.TipoPessoa == tipo).ToList();
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    // Admin vê professores por padrão
                    listaEntities = listaPessoas.Where(p => p.TipoPessoa == "P").ToList();
                }
                else
                {
                    var cpfLogado = User.Identity?.Name;
                    
                    if (string.IsNullOrEmpty(cpfLogado))
                         return RedirectToAction("Index", "Home");// No futuro aqui vai ser o dashboard

                    var pessoa = _pessoaService.GetByCpf(cpfLogado);
                    
                    if (pessoa != null)
                        return RedirectToAction(nameof(Details), new { id = pessoa.Id });
                    
                    return RedirectToAction("Index", "Home");
                }
            }

            var listaModels = _mapper.Map<List<PessoaModel>>(listaEntities);
            ViewBag.TipoAtual = tipo; 
            return View(listaModels);
        }
        
        // GET: PessoaController/Details/5
        public ActionResult Details(int id)
        {
 
                var entity = _pessoaService.Get(id);
                var model = _mapper.Map<PessoaModel>(entity);
                return View(model);
        }
        
        // GET: PessoaController/Create
        public ActionResult Create(string tipo)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (string.IsNullOrEmpty(tipo) || (tipo != "P" && tipo != "A" && tipo != "R"))
            {
                return RedirectToAction("Index", "Home"); 
            }
            
            // Só Admin ou Responsavel podem criar Aluno
            if (tipo == "A" && !User.IsInRole("Admin") && !User.IsInRole("Responsavel"))
            {
                return RedirectToAction("Index", "Home");
            }

            var cpfLogado = User.Identity.Name;
            var model = new PessoaModel
            {
                TipoPessoa = tipo
            };

            if (tipo == "A")
            {
                // Se for aluno, o responsavel ira cadastrar
                var pai = _pessoaService.GetByCpf(cpfLogado);
                if (pai != null)
                {
                    model.ResponsavelId = pai.Id;
                }
            }
            else
            {
                var pessoaExistente = _pessoaService.GetByCpf(cpfLogado);
                if (pessoaExistente != null)
                {
                    return RedirectToAction(nameof(Edit), new { id = pessoaExistente.Id });
                }
                // Preenche CPF e Email automaticamente da autenticação
                model.Cpf = cpfLogado;
                model.Email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            }
            
            CarregarViewBags(tipo);
            return View(model);
        }

        // POST: PessoaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PessoaModel model, IFormFile? arquivoDiploma, IFormFile? arquivoFoto, IFormFile? arquivoDocumento)
        {
            ValidarTamanhoArquivo(arquivoDiploma, "arquivoDiploma");
            ValidarTamanhoArquivo(arquivoFoto, "arquivoFoto");

            if (ModelState.IsValid)
            {
                    var pessoaEntity = _mapper.Map<Pessoa>(model);

                    pessoaEntity.Diploma = ConvertToBytes(arquivoDiploma);
                    pessoaEntity.FotoPerfil = ConvertToBytes(arquivoFoto);
                    pessoaEntity.FotoDocumento = ConvertToBytes(arquivoDocumento);
                    
                    _pessoaService.Create(pessoaEntity);

                    return RedirectToAction("Index", "Home");

            }

            CarregarViewBags(model.TipoPessoa);
            return View(model);
        }

        // GET: ProfessorController/Edit/5
        public ActionResult Edit(int id)
        {

                var entity = _pessoaService.Get(id);

                if (!User.IsInRole("Admin") && entity.Cpf != User.Identity?.Name)
                {
                    return RedirectToAction("Index", "Home");
                }

                var model = _mapper.Map<PessoaModel>(entity);
                CarregarViewBags(entity.TipoPessoa);
                return View(model);
        }

        // POST: PessoaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PessoaModel model, IFormFile? arquivoDiploma, IFormFile? arquivoFoto, IFormFile? arquivoDocumento)
        {
            if (ModelState.IsValid)
            {
                var pessoaEntity = _mapper.Map<Pessoa>(model);

                if (arquivoDiploma != null) pessoaEntity.Diploma = ConvertToBytes(arquivoDiploma);
                if (arquivoFoto != null) pessoaEntity.FotoPerfil = ConvertToBytes(arquivoFoto);
                if (arquivoDocumento != null) pessoaEntity.FotoDocumento = ConvertToBytes(arquivoDocumento);

                _pessoaService.Edit(pessoaEntity);

                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            CarregarViewBags(model.TipoPessoa);
            return View(model);
        }

        // GET: PessoaController/Delete/5
        public ActionResult Delete(int id)
        {

                var entity = _pessoaService.Get(id);
                
                if (!User.IsInRole("Admin") && entity.Cpf != User.Identity?.Name)
                {
                    // Redireciona silenciosamente se não for o dono
                    return RedirectToAction("Index", "Home");
                }
                
                var model = _mapper.Map<PessoaModel>(entity);
                return View(model);
        }
        
        // POST: DisciplinaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var entity = _pessoaService.Get(id);
            
            if (!User.IsInRole("Admin") && entity.Cpf != User.Identity?.Name)
            {
                return RedirectToAction("Index", "Home");
            }
            _pessoaService.Delete(id);
            
            return RedirectToAction(nameof(Index));
        }


        // MÉTODOS AUXILIARES
        private void CarregarViewBags(string? tipo)
        //Filtra as listas de cidades, disciplinas e responsáveis conforme o tipo de pessoa
        {
            if (tipo == "P") // Professor
            {
                ViewBag.ListaDeCidades = new SelectList(_cidadeService.GetAll(), "Id", "Nome");
                ViewBag.ListaDeDisciplinas = new SelectList(_disciplinaService.GetAll(), "Id", "Nome");
            }
            
            else if (tipo == "R") // Responsável
            {
                ViewBag.ListaDeCidades = new SelectList(_cidadeService.GetAll(), "Id", "Nome");
            }
        }

        private void ValidarTamanhoArquivo(IFormFile? file, string fieldName)
        {
            const long maxSizeBytes = 64 * 1024; // ~64KB algo perto disso, é isso que o tipo BLOB aceita!
            if (file != null && file.Length > maxSizeBytes)
            {
                ModelState.AddModelError(fieldName, $"O arquivo excede o limite de 64KB.");
            }
        }

        //Faz a conversão do arquivo enviado para um array de bytes, já que a view não aceita byte[]!
        private byte[]? ConvertToBytes(IFormFile? file)
        {
            if (file == null) return null;
            
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}