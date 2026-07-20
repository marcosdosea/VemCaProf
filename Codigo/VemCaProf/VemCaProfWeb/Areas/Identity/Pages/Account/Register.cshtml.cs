    // Licensed to the .NET Foundation under one or more agreements.
    // The .NET Foundation licenses this file to you under the MIT license.
    #nullable disable

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Logging;
    using VemCaProfWeb.Areas.Identity.Data; 
    using Core.Service;
    using Util;

    namespace VemCaProfWeb.Areas.Identity.Pages.Account
    {
        [AllowAnonymous]
        public class RegisterModel : PageModel
        {
            private readonly SignInManager<Usuario> _signInManager;
            private readonly UserManager<Usuario> _userManager;
            private readonly IUserStore<Usuario> _userStore;
            private readonly IUserEmailStore<Usuario> _emailStore;
            private readonly ILogger<RegisterModel> _logger;
            private readonly IEmailSender _emailSender;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly IPessoaService _pessoaService;

            public RegisterModel(
                UserManager<Usuario> userManager,
                IUserStore<Usuario> userStore,
                SignInManager<Usuario> signInManager,
                ILogger<RegisterModel> logger,
                IEmailSender emailSender,
                RoleManager<IdentityRole> roleManager,
                IPessoaService pessoaService)
            {
                _userManager = userManager;
                _userStore = userStore;
                _emailStore = GetEmailStore();
                _signInManager = signInManager;
                _logger = logger;
                _emailSender = emailSender;
                _roleManager = roleManager;
                _pessoaService = pessoaService;
            }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [BindProperty]
            public InputModel Input { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            public string ReturnUrl { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            public IList<AuthenticationScheme> ExternalLogins { get; set; }
            
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [BindProperty(SupportsGet = true)]
            public int? AlunoId { get; set; }
            
            public class InputModel
            {
                /// <summary>
                ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
                ///     directly from your code. This API may change or be removed in future releases.
                /// </summary>
                [Required]
                [Display(Name = "CPF")]
                public string Cpf { get; set; }

                /// <summary>
                ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
                ///     directly from your code. This API may change or be removed in future releases.
                /// </summary>
                [Required]
                [EmailAddress]
                [Display(Name = "Email")]
                public string Email { get; set; }

                /// <summary>
                ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
                ///     directly from your code. This API may change or be removed in future releases.
                /// </summary>
                [Required]
                [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
                [DataType(DataType.Password)]
                [Display(Name = "Senha")]
                public string Password { get; set; }
                
                /// <summary>
                ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
                ///     directly from your code. This API may change or be removed in future releases.
                /// </summary>
                [DataType(DataType.Password)]
                [Display(Name = "Confirmar senha")]
                [Compare("Password", ErrorMessage = "A senha e a confirmação não conferem.")]
                public string ConfirmPassword { get; set; }
                
                
                [Required(ErrorMessage = "Por favor, selecione um perfil.")]
                [Display(Name = "Tipo de Usuário")]
                public string TipoUsuario { get; set; }
            }

            public async Task OnGetAsync(string returnUrl = null, int? alunoId = null)
            {
                ReturnUrl = returnUrl;
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                if (alunoId.HasValue)
                {
                    AlunoId = alunoId; // Seta a propriedade para a View saber que é Modo Aluno
            
                    // Buscamos o aluno para pegar o CPF dele
                    // Use o seu Service para garantir que o Pai logado tem permissão de ver esse aluno
                    var cpfLogado = User.Identity?.Name;
                    bool isAdmin = User.IsInRole("Admin");
                    var aluno = _pessoaService.GetParaDetails(alunoId.Value, cpfLogado, isAdmin);

                    if (aluno != null)
                    {
                        // Preenchemos o Input com o CPF do aluno para ele aparecer na tela
                        Input = new InputModel
                        {
                            Cpf = aluno.Cpf // O CPF do aluno agora vai para a "caixinha" na tela
                        };
                    }
                }
            }

            public async Task<IActionResult> OnPostAsync(string returnUrl = null)
            {
                returnUrl ??= Url.Content("~/");
                ReturnUrl = returnUrl;
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                Input.Cpf = Methods.RemoveNaoNumericos(Input.Cpf);

                // ==========================================================
                // BLINDAGEM CONTRA ESCALAÇÃO DE PRIVILÉGIOS E VALIDAÇÃO MANUAL
                // ==========================================================
                if (AlunoId.HasValue)
                {
                    // Remove a exigência de selecionar Perfil, já que para aluno a Role é fixa
                    ModelState.Remove("Input.TipoUsuario");
                }
                if (!AlunoId.HasValue)
                {
                    // Apenas estes perfis podem se registrar pela rota pública
                    string[] rolesPermitidas = { "Professor", "Responsavel" };

                    // Evita que um espaço acidental (ex: " Professor ") quebre a validação
                    Input.TipoUsuario = Input.TipoUsuario?.Trim();

                    // Verifica se é nulo/vazio OU se tentaram injetar uma role fora da Lista Branca (ignorando maiúsculas/minúsculas)
                    if (string.IsNullOrEmpty(Input.TipoUsuario) || 
                        !rolesPermitidas.Contains(Input.TipoUsuario, StringComparer.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("Input.TipoUsuario", "Por favor, selecione um perfil válido.");
                    }
                }

                // Se a validação acima falhar, o ModelState fica inválido e o código pula todo o bloco abaixo,
                // caindo naturalmente no return Page() no final do método.
                if (ModelState.IsValid)
                {
                    // =========================================================
                    // MODO ALUNO: Pai/Admin criando acesso para um dependente
                    // =========================================================
                    if (AlunoId.HasValue)
                    {
                        // 1) Exigir autenticado e role
                        if (!(User?.Identity?.IsAuthenticated ?? false))
                            return Forbid();

                        bool isAdmin = User.IsInRole("Admin");
                        bool isResp = User.IsInRole("Responsavel");
                        if (!isAdmin && !isResp)
                            return Forbid();

                        var cpfLogado = User.Identity?.Name;

                        // 2) Validar que o aluno pertence a esse pai (ou admin)
                        var alunoOriginal = _pessoaService.GetParaDetails(AlunoId.Value, cpfLogado, isAdmin);
                        if (alunoOriginal == null || alunoOriginal.TipoPessoa != "A")
                            return Forbid();

                        // 3) CPF DEFINITIVO vem do banco (ignora qualquer CPF do HTML)
                        var cpfDefinitivo = alunoOriginal.Cpf;

                        // 4) Bloquear duplicidade
                        var existingByCpf = await _userManager.FindByNameAsync(cpfDefinitivo);
                        if (existingByCpf != null)
                        {
                            ModelState.AddModelError(string.Empty, "Este aluno já possui acesso.");
                            return Page();
                        }

                        var existingByEmail = await _userManager.FindByEmailAsync(Input.Email);
                        if (existingByEmail != null)
                        {
                            ModelState.AddModelError(nameof(Input.Email), "Este email já está em uso.");
                            return Page();
                        }

                        // 5) Criar usuário do Identity
                        var alunoUser = CreateUser();

                        await _userStore.SetUserNameAsync(alunoUser, cpfDefinitivo, CancellationToken.None);
                        await _emailStore.SetEmailAsync(alunoUser, Input.Email, CancellationToken.None);

                        // Se seu app exige confirmação de conta, isso evita travar o login do aluno
                        alunoUser.EmailConfirmed = true;

                        var alunoResult = await _userManager.CreateAsync(alunoUser, Input.Password);

                        if (alunoResult.Succeeded)
                        {
                            // A role "Aluno" está hardcoded, impossível de ser burlada
                            await _userManager.AddToRoleAsync(alunoUser, "Aluno");
                            
                            _pessoaService.AtualizarEmailPessoa(AlunoId.Value, Input.Email);
                            
                            // NÃO logar aqui (pai não vira aluno)
                            return RedirectToAction("Index", "Home");
                        }

                        foreach (var error in alunoResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return Page();
                    }

                    // =========================================================
                    // REGISTER NORMAL: usuário criando sua própria conta
                    // =========================================================

                    // --- NOVA VALIDAÇÃO: e-mail já existe na tabela Pessoa? ---
                    var pessoaExistente = _pessoaService.GetByEmail(Input.Email);
                    if (pessoaExistente != null)
                    {
                        ModelState.AddModelError(nameof(Input.Email), "Este e-mail já está vinculado a um perfil existente. Use outro e-mail.");
                        return Page();
                    }

                    var user = CreateUser();

                    await _userStore.SetUserNameAsync(user, Input.Cpf, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        // ROLES: Totalmente seguro, pois a whitelist lá em cima já garantiu o valor correto
                        await _userManager.AddToRoleAsync(user, Input.TipoUsuario);

                        // Confirmação de email (se você usa esse fluxo)
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false);

                        string tipoSigla = Input.TipoUsuario switch
                        {
                            "Professor" => "P",
                            "Responsavel" => "R",
                            _ => ""
                        };

                        return RedirectToAction("Create", "Pessoa", new { tipo = tipoSigla });
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                return Page();
            }

            private Usuario CreateUser()
            {
                try
                {
                    return Activator.CreateInstance<Usuario>();
                }
                catch
                {
                    throw new InvalidOperationException($"Can't create an instance of '{nameof(Usuario)}'. " +
                        $"Ensure that '{nameof(Usuario)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                        $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
                }
            }

            private IUserEmailStore<Usuario> GetEmailStore()
            {
                if (!_userManager.SupportsUserEmail)
                {
                    throw new NotSupportedException("The default UI requires a user store with email support.");
                }
                return (IUserEmailStore<Usuario>)_userStore;
            }
        }
    }
