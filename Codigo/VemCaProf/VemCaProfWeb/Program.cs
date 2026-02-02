using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;
using Microsoft.AspNetCore.Identity;
using VemCaProfWeb.Areas.Identity.Data;

namespace VemCaProfWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. SERVIÇOS
        builder.Services.AddControllersWithViews();
        
        // Injeções de Dependência
        builder.Services.AddTransient<IDisciplinaService, DisciplinaService>();
        builder.Services.AddTransient<ICidadeService, CidadeService>();
        builder.Services.AddTransient<IDisponibilidadeHorarioService, DisponibilidadeHorarioService>();
        builder.Services.AddTransient<IPessoaService, PessoaService>();
        builder.Services.AddTransient<IPenalidadeService, PenalidadeService>();

        // AutoMapper
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        // Banco de Dados
        var connectionString = builder.Configuration.GetConnectionString("VemCaProfConnection");
        var connectionStringIdentity = builder.Configuration.GetConnectionString("IdentityDatabase") ?? throw new InvalidOperationException("Connection string 'IdentityDatabase' not found.");   
        
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("A string de conexão 'VemCaProfConnection' não foi encontrada.");

        // Configuração dos Bancos
        builder.Services.AddDbContext<VemCaProfContext>(options =>
            options.UseMySQL(connectionString));

        builder.Services.AddDbContext<IdentityContext>(options =>
            options.UseMySQL(connectionStringIdentity));

        // --- CONFIGURAÇÃO DO IDENTITY (APENAS UM BLOCO AQUI) ---
        builder.Services.AddIdentity<Usuario, IdentityRole>(options => 
            {
                options.SignIn.RequireConfirmedAccount = false; 
                options.Password.RequireDigit = false;          
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();
        
        // 3. PIPELINE
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        
        // Ordem correta dos Middlewares
        app.UseAuthentication(); 
        app.UseAuthorization();

        app.MapRazorPages();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}