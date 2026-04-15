using Microsoft.EntityFrameworkCore;

namespace Admission.Dictionary.Persistence;

public static class DictionaryExtensions
{
    public static void AddDictionaryPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Dictionary");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'Dictionary' is missing. Configure it in appsettings or environment variables.");
        }

        services.AddDbContext<DictionaryDbContext>(options => options.UseNpgsql(connectionString));
    }
}
