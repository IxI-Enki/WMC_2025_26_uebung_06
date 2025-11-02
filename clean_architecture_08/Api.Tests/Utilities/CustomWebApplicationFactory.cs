using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence.Repositories;
using Application.Services;
using FluentValidation;
using Domain.Contracts;
using Application.Contracts.Repositories;
using Application.Contracts;

namespace Api.Tests.Utilities;

/// <summary>
/// Custom WebApplicationFactory for integration tests with in-memory database.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Skip Infrastructure registration from Program.cs (we configure it here instead)
        builder.UseSetting("SkipInfrastructure", "true");

        builder.ConfigureServices(services =>
        {
            // Remove previously registered DbContext (if any)
            var toRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(AppDbContext)).ToList();
            foreach (var d in toRemove) services.Remove(d);

            // Use in-memory database for tests
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("DevicesApiTestsDb"));

            // Register repositories & UoW (mimic Infrastructure.AddInfrastructure, but without SQL / seeder)
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IUsageRepository, UsageRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register domain services
            services.AddScoped<IPersonUniquenessChecker, PersonUniquenessChecker>();
            services.AddScoped<IUsageOverlapChecker, UsageOverlapChecker>();

            // Add Application layer MediatR + Validators if not already (idempotent)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IUnitOfWork).Assembly));
            services.AddValidatorsFromAssembly(typeof(IUnitOfWork).Assembly);

            // Build provider & ensure db created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }
}

