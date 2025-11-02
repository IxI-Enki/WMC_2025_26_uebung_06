using Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Application.Pipeline;
using Domain.Contracts;
using Application.Contracts;

namespace Application;

/// <summary>
/// Erweiterungsmethoden für DI-Registrierung der Application-Dienste.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registriert MediatR, FluentValidation, Pipeline Behaviors und Domain Services.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // CQRS + MediatR + FluentValidation
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(IUnitOfWork).Assembly); // Application-Assembly
        });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssembly(typeof(IUnitOfWork).Assembly);

        // Domain Services
        services.AddScoped<IPersonUniquenessChecker, PersonUniquenessChecker>();
        services.AddScoped<IUsageOverlapChecker, UsageOverlapChecker>();

        return services;
    }
}

