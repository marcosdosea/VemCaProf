using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using VemCaProfWeb;

namespace VemCaProfWebTests;

[TestClass]
public class AuthorizationTests
{
    private WebApplicationFactory<Program> _factory = null!;

    [TestInitialize]
    public void Initialize()
    {
        _factory = new WebApplicationFactory<Program>();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _factory.Dispose();
    }

    [DataTestMethod]
    [DataRow("/")]
    [DataRow("/Home/Privacy")]
    [DataRow("/Aula")]
    [DataRow("/Cidade")]
    [DataRow("/DisponibilidadeHorario")]
    [DataRow("/Disciplina")]
    [DataRow("/Pagamento")]
    [DataRow("/Penalidade")]
    [DataRow("/Pessoa")]
    public async Task AnonymousUser_IsRedirectedToLogin(string url)
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync(url);

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual("/Identity/Account/Login", response.Headers.Location?.AbsolutePath);
        StringAssert.StartsWith(response.Headers.Location?.Query, "?ReturnUrl=");
    }

    [TestMethod]
    public async Task LoginPage_IsAvailableAnonymously()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Identity/Account/Login");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task AdminLayout_RendersSuccessfully()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Home/Error");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task StaticFiles_AreAvailableAnonymously()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/css/site.css");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
