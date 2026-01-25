using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Configuration;
using Core.Service;
using Service;

namespace VemCaProfWeb
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //injeção de dependência DBContext
            var connectionString = Configuration.GetConnectionString("VemCaProfConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("A string de conexão 'VemCaProfConnection' não foi encontrada ou está vazia.");
            _ = services.AddDbContext<VemCaProfContext>(options =>
                options.UseMySQL(connectionString));

            services.AddTransient<IDisciplinaService, DisciplinaService>();

            services.AddAutoMapper(typeof(Startup).Assembly);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
