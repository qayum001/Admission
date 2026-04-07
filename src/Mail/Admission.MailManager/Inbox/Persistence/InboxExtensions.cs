using Microsoft.EntityFrameworkCore;

namespace Admission.MailManager.Inbox.Persistence;

public static class InboxExtensions
{
    public static void AddInboxPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MailInbox");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'MailInbox' is missing. Configure it in appsettings or environment variables.");
        }

        services.AddDbContext<InboxDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IInboxMessageWriter, EfCoreInboxMessageWriter>();
    }
}
