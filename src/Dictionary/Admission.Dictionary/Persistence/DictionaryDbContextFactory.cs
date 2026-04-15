using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Admission.Dictionary.Persistence;

public sealed class DictionaryDbContextFactory : IDesignTimeDbContextFactory<DictionaryDbContext>
{
    public DictionaryDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Dictionary")
            ?? throw new InvalidOperationException("Connection string 'Dictionary' is missing.");

        var optionsBuilder = new DbContextOptionsBuilder<DictionaryDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new DictionaryDbContext(optionsBuilder.Options);
    }
}
