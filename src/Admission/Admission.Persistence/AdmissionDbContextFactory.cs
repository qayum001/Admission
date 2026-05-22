using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Admission.Persistence;

public sealed class AdmissionDbContextFactory : IDesignTimeDbContextFactory<AdmissionDbContext>
{
    public AdmissionDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Admission")
            ?? throw new InvalidOperationException("Connection string 'Admission' is missing.");

        var optionsBuilder = new DbContextOptionsBuilder<AdmissionDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AdmissionDbContext(optionsBuilder.Options);
    }
}
