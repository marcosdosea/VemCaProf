using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace AutoTest
{
    [TestFixture]
    [NonParallelizable]
    public class PenalidadeAutoTest
    {
        private const string BaseUrl = "https://localhost:7266";
        private Process? webProcess;
        private IWebDriver driver = null!;
        private IJavaScriptExecutor js = null!;

        [OneTimeSetUp]
        public void IniciarAplicacao()
        {
            if (ServidorDisponivel())
                return;

            var solutionDirectory = Path.GetFullPath(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "..", "..", "..", ".."));
            var webProject = Path.Combine(solutionDirectory, "VemCaProfWeb", "VemCaProfWeb.csproj");
            Assert.That(File.Exists(webProject), Is.True, $"Projeto web não encontrado em {webProject}.");

            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = solutionDirectory,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add("run");
            startInfo.ArgumentList.Add("--project");
            startInfo.ArgumentList.Add(webProject);
            startInfo.ArgumentList.Add("--launch-profile");
            startInfo.ArgumentList.Add("https");

            webProcess = Process.Start(startInfo);
            Assert.That(webProcess, Is.Not.Null, "Não foi possível iniciar a aplicação web.");

            var limite = DateTime.UtcNow.AddSeconds(30);
            while (DateTime.UtcNow < limite && !ServidorDisponivel())
            {
                if (webProcess!.HasExited)
                    Assert.Fail($"A aplicação web encerrou durante a inicialização. Código: {webProcess.ExitCode}.");

                Thread.Sleep(500);
            }

            Assert.That(ServidorDisponivel(), Is.True, $"A aplicação não respondeu em {BaseUrl}.");
        }

        [OneTimeTearDown]
        public void EncerrarAplicacao()
        {
            if (webProcess is { HasExited: false })
                webProcess.Kill(entireProcessTree: true);

            webProcess?.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            driver = new ChromeDriver();
            js = (IJavaScriptExecutor)driver;

            driver.Navigate().GoToUrl($"{BaseUrl}/Identity/Account/Login");
            driver.FindElement(By.Id("Input_Email")).SendKeys("admin1@email.com");
            driver.FindElement(By.Id("Input_Password")).SendKeys("123456");
            driver.FindElement(By.Id("login-submit")).Click();

            Aguardar(d => !d.Url.Contains("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase));
        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Test]
        public void CadastroComSucesso()
        {
            var tipo = $"Cadastro-{Guid.NewGuid():N}"[..17];
            var inicio = DateTime.Today.AddYears(1).AddHours(11);
            var fim = inicio.AddHours(1);

            CriarPenalidade(tipo, "Cadastro automatizado", inicio, fim);

            try
            {
                var celulas = EncontrarLinha(tipo).FindElements(By.TagName("td"));
                Assert.Multiple(() =>
                {
                    Assert.That(celulas[0].Text, Is.EqualTo(inicio.ToString("dd/MM/yyyy HH:mm")));
                    Assert.That(celulas[1].Text, Is.EqualTo(fim.ToString("dd/MM/yyyy HH:mm")));
                    Assert.That(celulas[2].Text, Is.EqualTo(tipo));
                    Assert.That(celulas[3].Text, Is.EqualTo("Cadastro automatizado"));
                    Assert.That(celulas[4].Text, Is.Not.Empty);
                    Assert.That(celulas[5].Text, Is.Not.Empty);
                });
            }
            finally
            {
                ExcluirSeExistir(tipo);
            }
        }

        [Test]
        public void EditarComSucesso()
        {
            var tipoOriginal = $"Editar-{Guid.NewGuid():N}"[..15];
            var tipoEditado = $"Editado-{Guid.NewGuid():N}"[..16];
            var inicio = DateTime.Today.AddYears(1).AddHours(13);
            var fim = inicio.AddHours(1);
            CriarPenalidade(tipoOriginal, "Antes da edicao", inicio, fim);

            try
            {
                NavegarPeloLink(EncontrarLinha(tipoOriginal).FindElement(By.CssSelector(".edit-icon")));
                Aguardar(d => d.FindElements(By.Id("Tipo")).Count == 1);

                var novoInicio = inicio.AddDays(1);
                var novoFim = novoInicio.AddHours(2);
                DefinirData("DataHorarioInicio", novoInicio);
                DefinirData("DataHoraFim", novoFim);
                PreencherCampo("Tipo", tipoEditado);
                PreencherCampo("Descricao", "Depois da edicao");
                var descricaoEnviada = driver.FindElement(By.Id("Descricao")).GetAttribute("value");
                js.ExecuteScript("arguments[0].click();", driver.FindElement(By.CssSelector("input[type='submit']")));
                try
                {
                    AguardarTabela();
                }
                catch (WebDriverTimeoutException)
                {
                    var erros = driver.FindElements(By.CssSelector(".text-danger"))
                        .Select(e => e.Text)
                        .Where(t => !string.IsNullOrWhiteSpace(t));
                    Assert.Fail($"Edição não retornou à listagem. URL: {driver.Url}. Descrição enviada: {descricaoEnviada}. Erros: {string.Join(" | ", erros)}");
                }

                var celulas = EncontrarLinha(tipoEditado).FindElements(By.TagName("td"));
                Assert.Multiple(() =>
                {
                    Assert.That(celulas[0].Text, Is.EqualTo(novoInicio.ToString("dd/MM/yyyy HH:mm")));
                    Assert.That(celulas[1].Text, Is.EqualTo(novoFim.ToString("dd/MM/yyyy HH:mm")));
                    Assert.That(celulas[2].Text, Is.EqualTo(tipoEditado));
                    Assert.That(celulas[3].Text, Is.EqualTo("Depois da edicao"));
                });
            }
            finally
            {
                ExcluirSeExistir(tipoEditado);
                ExcluirSeExistir(tipoOriginal);
            }
        }

        [Test]
        public void ExcluirComSucesso()
        {
            var tipo = $"Excluir-{Guid.NewGuid():N}"[..16];
            var inicio = DateTime.Today.AddYears(1).AddHours(15);
            CriarPenalidade(tipo, "Exclusao automatizada", inicio, inicio.AddHours(1));

            NavegarPeloLink(EncontrarLinha(tipo).FindElement(By.CssSelector(".delete-icon")));
            Aguardar(d => d.FindElements(By.CssSelector("form .btn-danger")).Count == 1);
            Assert.That(driver.FindElement(By.TagName("h3")).Text, Is.EqualTo("Tem certeza que deseja excluir?"));

            SubmeterFormularioDoBotao("form .btn-danger");
            AguardarTabela();

            Assert.That(Linhas().Any(l => TipoDaLinha(l) == tipo), Is.False);
        }

        private void CriarPenalidade(string tipo, string descricao, DateTime inicio, DateTime fim)
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Penalidade/Create");
            Aguardar(d => d.FindElements(By.Id("DataHorarioInicio")).Count == 1);

            DefinirData("DataHorarioInicio", inicio);
            DefinirData("DataHoraFim", fim);
            PreencherCampo("Tipo", tipo);
            PreencherCampo("Descricao", descricao);
            SelecionarPrimeiraOpcao("IdProfessor");
            SelecionarPrimeiraOpcao("IdResponsavel");
            js.ExecuteScript("arguments[0].submit();", driver.FindElement(By.CssSelector("form.penalidade-form")));
            try
            {
                AguardarTabela();
            }
            catch (WebDriverTimeoutException)
            {
                var erros = driver.FindElements(By.CssSelector(".text-danger"))
                    .Select(e => e.Text)
                    .Where(t => !string.IsNullOrWhiteSpace(t));
                var valores = new[] { "DataHorarioInicio", "DataHoraFim", "Tipo", "Descricao", "IdProfessor", "IdResponsavel" }
                    .Select(id => $"{id}={driver.FindElement(By.Id(id)).GetAttribute("value")}");
                Assert.Fail($"Cadastro não retornou à listagem. URL: {driver.Url}. Erros: {string.Join(" | ", erros)}. Valores: {string.Join(" | ", valores)}");
            }
        }

        private void DefinirData(string id, DateTime data)
        {
            var campo = driver.FindElement(By.Id(id));
            js.ExecuteScript(
                "arguments[0].value = arguments[1]; arguments[0].dispatchEvent(new Event('change', { bubbles: true }));",
                campo,
                data.ToString("yyyy-MM-ddTHH:mm"));
        }

        private void PreencherCampo(string id, string valor)
        {
            var campo = driver.FindElement(By.Id(id));
            js.ExecuteScript(
                "arguments[0].value = arguments[1]; arguments[0].dispatchEvent(new Event('input', { bubbles: true })); arguments[0].dispatchEvent(new Event('change', { bubbles: true }));",
                campo,
                valor);
        }

        private void SelecionarPrimeiraOpcao(string id)
        {
            var opcoes = driver.FindElement(By.Id(id)).FindElements(By.TagName("option"));
            Assert.That(opcoes, Has.Count.GreaterThan(1), $"O campo {id} não possui opções cadastradas.");
            opcoes[1].Click();
        }

        private IWebElement EncontrarLinha(string tipo)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(
                _ => Linhas().FirstOrDefault(l => TipoDaLinha(l) == tipo))!;
        }

        private IReadOnlyCollection<IWebElement> Linhas()
        {
            return driver.FindElements(By.CssSelector("tbody tr"));
        }

        private static string? TipoDaLinha(IWebElement linha)
        {
            var celulas = linha.FindElements(By.TagName("td"));
            return celulas.Count > 2 ? celulas[2].Text : null;
        }

        private void ExcluirSeExistir(string tipo)
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Penalidade");
            AguardarTabela();
            var linha = Linhas().FirstOrDefault(l => TipoDaLinha(l) == tipo);
            if (linha == null)
                return;

            NavegarPeloLink(linha.FindElement(By.CssSelector(".delete-icon")));
            Aguardar(d => d.FindElements(By.CssSelector("form .btn-danger")).Count == 1);
            SubmeterFormularioDoBotao("form .btn-danger");
            AguardarTabela();
        }

        private void AguardarTabela()
        {
            Aguardar(d => d.FindElements(By.CssSelector("table.penalidades-table")).Count == 1);
        }

        private void NavegarPeloLink(IWebElement link)
        {
            driver.Navigate().GoToUrl(link.GetAttribute("href")!);
        }

        private void SubmeterFormularioDoBotao(string seletor)
        {
            var formulario = driver.FindElement(By.CssSelector(seletor))
                .FindElement(By.XPath("./ancestor::form"));
            js.ExecuteScript("arguments[0].submit();", formulario);
        }

        private void Aguardar(Func<IWebDriver, bool> condicao)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(condicao);
        }

        private static bool ServidorDisponivel()
        {
            try
            {
                using var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(1) };
                using var response = client.GetAsync(BaseUrl).GetAwaiter().GetResult();
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }
    }
}
