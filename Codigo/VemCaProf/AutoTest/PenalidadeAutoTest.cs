using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace AutoTest
{
    [TestFixture]
    public class PenalidadeAutoTest
    {
        private IWebDriver driver;
        public IDictionary<string, object> vars { get; private set; }
        private IJavaScriptExecutor js;
        [SetUp]
        public void SetUp()
        {
            driver = new ChromeDriver();
            js = (IJavaScriptExecutor)driver;
            vars = new Dictionary<string, object>();
        }
        [TearDown]
        public void Teardown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose(); 
                driver = null;
            }
        }
        [Test]
        public void ExecutarCadastroComSucesso()
        {
            driver.Navigate().GoToUrl("https://localhost:7266/");
            driver.Manage().Window.Size = new System.Drawing.Size(1552, 832);
            driver.FindElement(By.CssSelector(".nav-item:nth-child(6) span")).Click();
            driver.FindElement(By.CssSelector(".pb-3")).Click();
            Assert.That(driver.FindElement(By.CssSelector(".secondary-text")).Text, Is.EqualTo("Histórico de penalidades"));
            driver.FindElement(By.LinkText("Criar Nova Penalidade")).Click();
            // --- INJEÇÃO DEFINITIVA VIA JAVASCRIPT ---

            // 1. Define a Data de Início no formato que o HTML5 exige internamente (yyyy-MM-ddTHH:mm)
            var campoInicio = driver.FindElement(By.Id("DataHorarioInicio"));
            this.js.ExecuteScript("arguments[0].value = '2027-10-20T11:50';", campoInicio);

            // 2. Define a Data de Fim no mesmo formato interno
            var campoFim = driver.FindElement(By.Id("DataHoraFim"));
            this.js.ExecuteScript("arguments[0].value = '2027-10-28T11:50';", campoFim);
            driver.FindElement(By.Id("Tipo")).Click();
            driver.FindElement(By.Id("Tipo")).SendKeys("Atraso");
            driver.FindElement(By.Id("Descricao")).Click();
            driver.FindElement(By.Id("Descricao")).SendKeys("há descumprimento dos prazos");
            driver.FindElement(By.Id("IdProfessor")).Click();
            {
                var dropdown = driver.FindElement(By.Id("IdProfessor"));
                dropdown.FindElement(By.XPath("//option[. = 'João']")).Click();
            }
            driver.FindElement(By.Id("IdResponsavel")).Click();
            {
                var dropdown = driver.FindElement(By.Id("IdResponsavel"));
                dropdown.FindElement(By.XPath("//option[. = 'Roberto']")).Click();
            }

            var botaoSalvar = driver.FindElement(By.CssSelector("button.btn-salvar"));
            this.js.ExecuteScript("arguments[0].click();", botaoSalvar);
            
            Thread.Sleep(3000);


            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(5));

            // Espera até que pelo menos a tabela ou uma linha de dados exista na tela
            wait.Until(d => d.FindElement(By.CssSelector("table")));
            driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(1)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(1)")).Text, Is.EqualTo("20/10/2027 11:50"));
            driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(2)")).Click();
            driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(2)")).Click();
            {
                var element = driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(2)"));
                Actions builder = new Actions(driver);
                builder.DoubleClick(element).Perform();
            }
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(2)")).Text, Is.EqualTo("28/10/2027 11:50"));
            driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(3)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(3)")).Text, Is.EqualTo("Atraso"));
            driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(4)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(4)")).Text, Is.EqualTo("há descumprimento dos prazos"));
            driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(5)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(5)")).Text, Is.EqualTo("João"));
            driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(6)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(4) > td:nth-child(6)")).Text, Is.EqualTo("Roberto"));
        }
    }
}