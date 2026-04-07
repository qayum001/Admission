using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Admission.MailManager.Inbox.Persistence;

public static class InboxMigrationExtensions
{
    public static async Task ApplyInboxMigrationsAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        const int maxAttempts = 10;
        var delay = TimeSpan.FromSeconds(3);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<InboxDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<InboxDbContext>>();

                logger.LogInformation("Applying inbox migrations");
                await dbContext.Database.MigrateAsync(cancellationToken);
                logger.LogInformation("Inbox migrations applied");

                return;
            }
            catch (Exception ex) when (ex is NpgsqlException or TimeoutException && attempt < maxAttempts)
            {
                using var scope = app.Services.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<InboxDbContext>>();

                logger.LogWarning(
                    ex,
                    "Failed to apply inbox migrations on attempt {Attempt} of {MaxAttempts}. Retrying in {DelaySeconds} seconds",
                    attempt,
                    maxAttempts,
                    delay.TotalSeconds);

                await Task.Delay(delay, cancellationToken);
            }
        }

        using var finalScope = app.Services.CreateScope();
        var finalDbContext = finalScope.ServiceProvider.GetRequiredService<InboxDbContext>();
        await finalDbContext.Database.MigrateAsync(cancellationToken);
    }
}
