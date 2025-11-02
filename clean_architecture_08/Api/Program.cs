using Application;
using Infrastructure;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace Api
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var skipInfrastructure = builder.Configuration["SkipInfrastructure"]?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
            // Pfad zur CSV-Datei (im Ausgabeverzeichnis erwartet)
            var csvPath = Path.Combine(AppContext.BaseDirectory, "data.csv");
            // DB-Connection aus appsettings.json (Default). Fällt sonst auf LocalDB zurück (siehe Infrastructure.AddInfrastructure)
            var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new ArgumentException("Connection string not found");
            // Registriert Infrastruktur (DbContext, Repositories, UoW, CSV-Reader, Seeder)
            if (!skipInfrastructure)
            {
                builder.Services.AddInfrastructure(csvPath, connectionString);
            }
            builder.Services.AddApplication();
            // Web API Basics - mit String-Enum Unterstützung
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            // Swagger/OpenAPI für einfache Erkundung der API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Devices API",
                    Version = "v1",
                    Description = "API zur Verwaltung von Geräten, Personen und Nutzungen"
                });
            });
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Devices API v1");
                    c.RoutePrefix = "swagger";
                    c.DisplayRequestDuration();
                });
            }

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}
