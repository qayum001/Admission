using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Admission.Dictionary.Persistence;

public static class DictionaryMigrationExtensions
{
    public static async Task ApplyDictionaryMigrationsAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        const int maxAttempts = 10;
        var delay = TimeSpan.FromSeconds(3);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DictionaryDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<DictionaryDbContext>>();

                logger.LogInformation("Applying Dictionary migrations");
                await dbContext.Database.MigrateAsync(cancellationToken);
                logger.LogInformation("Dictionary migrations applied");

                return;
            }
            catch (Exception ex) when (ex is NpgsqlException or TimeoutException && attempt < maxAttempts)
            {
                using var scope = app.Services.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<DictionaryDbContext>>();

                logger.LogWarning(
                    ex,
                    "Failed to apply dictionary migrations on attempt {Attempt} of {MaxAttempts}. Retrying in {DelaySeconds} seconds",
                    attempt,
                    maxAttempts,
                    delay.TotalSeconds);

                await Task.Delay(delay, cancellationToken);
            }
        }

        using var finalScope = app.Services.CreateScope();
        var finalDbContext = finalScope.ServiceProvider.GetRequiredService<DictionaryDbContext>();
        await finalDbContext.Database.MigrateAsync(cancellationToken);
    }
}
