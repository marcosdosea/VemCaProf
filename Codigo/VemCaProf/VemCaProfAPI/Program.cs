
using Core;
using Core.Service;
using Mappers;
using Microsoft.EntityFrameworkCore;
using Service;
using VemCaProfWeb.Areas.Identity.Data;

namespace VemCaProfAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            


            // Add services to the container.

            builder.Services.AddControllers();

            //injeção de dependência
            builder.Services.AddTransient<IDisciplinaService, DisciplinaService>();
            builder.Services.AddTransient<ICidadeService, CidadeService>();
            builder.Services.AddTransient<IDisponibilidadeHorarioService, DisponibilidadeHorarioService>();
            builder.Services.AddTransient<IPessoaService, PessoaService>();
            builder.Services.AddTransient<IPenalidadeService, PenalidadeService>();
            builder.Services.AddTransient<IAulaService, AulaService>();
            builder.Services.AddTransient<IPagamentoService, PagamentoService>();

            // Configuração do AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddAutoMapper(typeof(PenalidadeProfile));
            builder.Services.AddAutoMapper(typeof(PenalidadeProfile).Assembly);

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

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
