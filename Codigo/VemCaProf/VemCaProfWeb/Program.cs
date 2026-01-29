using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

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
        builder.Services.AddTransient<IPenalidadeService,PenalidadeService>();

        // AutoMapper
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        // Banco de Dados
        var connectionString = builder.Configuration.GetConnectionString("VemCaProfConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("A string de conexão 'VemCaProfConnection' não foi encontrada.");

        // 2. BUILD
        builder.Services.AddDbContext<VemCaProfContext>(options =>
            options.UseMySQL(connectionString));
        
        // 3. PIPELINE
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();
        

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();

        
    }
}