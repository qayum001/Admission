using Microsoft.EntityFrameworkCore;

namespace Admission.Auth.Persistence;

public static class AuthPersistenceExtensions
{
    public static void AddAuthPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var authConnectionString = configuration.GetConnectionString("Auth");

        if (string.IsNullOrWhiteSpace(authConnectionString))
        {
            throw new InvalidOperationException("Connection string 'Auth' is missing. Configure it in appsettings or environment variables.");
        }

        services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(authConnectionString));
    }
}
