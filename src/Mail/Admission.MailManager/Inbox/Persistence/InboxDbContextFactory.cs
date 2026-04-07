using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Admission.MailManager.Inbox.Persistence;

public sealed class InboxDbContextFactory : IDesignTimeDbContextFactory<InboxDbContext>
{
    public InboxDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("MailInbox")
            ?? throw new InvalidOperationException("Connection string 'MailInbox' is missing.");

        var optionsBuilder = new DbContextOptionsBuilder<InboxDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new InboxDbContext(optionsBuilder.Options);
    }
}
