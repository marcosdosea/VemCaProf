using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Core;
using Core.Service;

namespace VemCaProfWeb.Filter
{
    /// <summary>
    /// Filtro de ação que verifica se o usuário autenticado possui um perfil (pessoa) cadastrado.
    /// Redireciona para a criação de perfil caso não exista, ou para criação de aluno caso o responsável
    /// ainda precise cadastrar dependentes.
    /// </summary>
    public class VerificaPerfilFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var controller = context.RouteData.Values["controller"]?.ToString();
                var action = context.RouteData.Values["action"]?.ToString();
                var area = context.RouteData.Values["area"]?.ToString();
                
                if (area == "Identity" ||
                    (controller == "Pessoa" && (action == "Create" || action == "Edit" || action == "Details")))
                {
                    await next();
                    return;
                }

                var pessoaService = context.HttpContext.RequestServices.GetRequiredService<IPessoaService>();
                var cpf = user.Identity!.Name;

                var perfil = pessoaService.GetByCpf(cpf);
                
                if (perfil == null)
                {
                    var tipoSigla = user.IsInRole("Professor") ? "P"
                        : user.IsInRole("Aluno") ? "A"
                        : user.IsInRole("Responsavel") ? "R"
                        : "";

                    context.Result = new RedirectToActionResult("Create", "Pessoa", new { tipo = tipoSigla });
                    return;
                }

                if (pessoaService.ResponsavelPrecisaCadastrarAlunos(cpf))
                {
                    context.Result = new RedirectToActionResult("Create", "Pessoa", new { tipo = "A" });
                    return;
                }
            }

            await next();
        }
    }
}