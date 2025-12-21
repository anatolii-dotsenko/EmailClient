using Microsoft.Extensions.DependencyInjection;
using EmailClient.Core.Interfaces;
using EmailClient.Infrastructure.Repositories;
using EmailClient.Infrastructure.Services;

namespace EmailClient.Infrastructure.Extensions;

/// <summary>
/// Extension methods for service collection configuration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure services to the DI container.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, MailKitEmailService>();
        services.AddScoped<IEmailStorage, JsonEmailStorage>();
        return services;
    }
}
