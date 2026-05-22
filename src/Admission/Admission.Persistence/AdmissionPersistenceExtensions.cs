using Admission.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Admission.Persistence;

public static class AdmissionPersistenceExtensions
{
    public static IServiceCollection AddAdmissionPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Admission");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'Admission' is missing. Configure it in appsettings or environment variables.");
        }

        services.AddScoped<DomainEventsInterceptor>();
        services.AddDbContext<AdmissionDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
            options.AddInterceptors(sp.GetRequiredService<DomainEventsInterceptor>());
        });
        services.AddScoped<IRepository>(sp => sp.GetRequiredService<AdmissionDbContext>());

        return services;
    }

    public static async Task ApplyAdmissionMigrationsAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AdmissionDbContext>();
        await db.Database.MigrateAsync();
    }
}
