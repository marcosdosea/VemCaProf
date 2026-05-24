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
        public void CadastroComSucesso()
        {
            driver.Navigate().GoToUrl("https://localhost:7266/");
            driver.Manage().Window.Size = new System.Drawing.Size(1552, 832);
            driver.FindElement(By.CssSelector(".nav-item:nth-child(6) span")).Click();
            driver.FindElement(By.CssSelector(".pb-3")).Click();
            Assert.That(driver.FindElement(By.CssSelector(".secondary-text")).Text, Is.EqualTo("Histórico de penalidades"));
            driver.FindElement(By.LinkText("Criar Nova Penalidade")).Click();
           

            
            var campoInicio = driver.FindElement(By.Id("DataHorarioInicio"));
            this.js.ExecuteScript("arguments[0].value = '2027-10-20T11:50';", campoInicio);

           
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
            
            


            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(5));

            
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

        [Test]
        public void EditarComSucesso()
        {
            driver.Navigate().GoToUrl("https://localhost:7266/");
            driver.Manage().Window.Size = new System.Drawing.Size(1552, 832);
            driver.FindElement(By.CssSelector(".nav-item:nth-child(6) span")).Click();
            driver.FindElement(By.CssSelector(".pb-3")).Click();
            Assert.That(driver.FindElement(By.CssSelector(".secondary-text")).Text, Is.EqualTo("Histórico de penalidades"));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(1)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(1)")).Text, Is.EqualTo("20/05/2026 08:00"));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(2)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(2)")).Text, Is.EqualTo("20/05/2026 09:00"));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(3)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(3)")).Text, Is.EqualTo("Advertência"));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(4)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(4)")).Text, Is.EqualTo("Atraso na entrega de atividade"));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(5)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(5)")).Text, Is.EqualTo("Maria"));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(6)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(6)")).Text, Is.EqualTo("Carlos"));
            {
                var element = driver.FindElement(By.CssSelector("tr:nth-child(1) .edit-icon"));
                Actions builder = new Actions(driver);
                builder.MoveToElement(element).ClickAndHold().Perform();
            }
            {
                var element = driver.FindElement(By.CssSelector("tr:nth-child(1) .edit-icon"));
                Actions builder = new Actions(driver);
                builder.MoveToElement(element).Perform();
            }
            {
                var element = driver.FindElement(By.CssSelector("tr:nth-child(1) .edit-icon"));
                Actions builder = new Actions(driver);
                builder.MoveToElement(element).Release().Perform();
            }
            var textoH1 = driver.FindElement(By.CssSelector("h1")).Text;
            Assert.That(textoH1, Is.EqualTo("Editar"));
            var textoH4 = driver.FindElement(By.CssSelector("h4:nth-child(2)")).Text;
            Assert.That(textoH4, Is.EqualTo("Penalidade: Advertência"));

            var campo = driver.FindElement(By.Id("DataHorarioInicio"));
            this.js.ExecuteScript("arguments[0].value = '2027-10-30T08:00';", campo);
            campo = driver.FindElement(By.Id("DataHoraFim"));
            this.js.ExecuteScript("arguments[0].value = '2027-11-12T09:00';", campo);

            campo = driver.FindElement(By.Id("Tipo"));
            campo.Clear();
            campo.SendKeys("Aviso");
            campo = driver.FindElement(By.Id("Descricao"));
            campo.Clear();
            campo.SendKeys("Avaliação não foi feita");
            driver.FindElement(By.Id("IdProfessor")).Click();
            {
                var dropdown = driver.FindElement(By.Id("IdProfessor"));
                dropdown.FindElement(By.XPath("//option[. = 'Ana']")).Click();
            }
            driver.FindElement(By.Id("IdResponsavel")).Click();
            {
                var dropdown = driver.FindElement(By.Id("IdResponsavel"));
                dropdown.FindElement(By.XPath("//option[. = 'Fernanda']")).Click();
            }
            var botaoSalvar = driver.FindElement(By.ClassName("btn-warning"));
            this.js.ExecuteScript("arguments[0].click();", botaoSalvar);

            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(5));

            
            wait.Until(d => d.FindElement(By.CssSelector("table")));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(1)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(1)")).Text, Is.EqualTo("30/10/2027 08:00"));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(2)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(2)")).Text, Is.EqualTo("12/11/2027 09:00"));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(3)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(3)")).Text, Is.EqualTo("Aviso"));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(4)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(4)")).Text, Is.EqualTo("Avaliação não foi feita"));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(5)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(5)")).Text, Is.EqualTo("Ana"));
            driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(6)")).Click();
            Assert.That(driver.FindElement(By.CssSelector("tr:nth-child(1) > td:nth-child(6)")).Text, Is.EqualTo("Fernanda"));
        }
    }
}