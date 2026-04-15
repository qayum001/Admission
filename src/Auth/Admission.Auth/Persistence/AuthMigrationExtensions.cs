using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Admission.Auth.Persistence;

public static class AuthMigrationExtensions
{
    public static async Task ApplyAuthMigrationsAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        await ApplyMigrationsWithRetryAsync<AuthDbContext>(app, "auth", cancellationToken);
    }

    private static async Task ApplyMigrationsWithRetryAsync<TDbContext>(
        WebApplication app,
        string migrationName,
        CancellationToken cancellationToken)
        where TDbContext : DbContext
    {
        const int maxAttempts = 10;
        var delay = TimeSpan.FromSeconds(3);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<TDbContext>>();

                logger.LogInformation("Applying {MigrationName} migrations", migrationName);
                await dbContext.Database.MigrateAsync(cancellationToken);
                logger.LogInformation("{MigrationName} migrations applied", migrationName);
                return;
            }
            catch (Exception ex) when (ex is NpgsqlException or TimeoutException && attempt < maxAttempts)
            {
                using var scope = app.Services.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<TDbContext>>();

                logger.LogWarning(
                    ex,
                    "Failed to apply {MigrationName} migrations on attempt {Attempt} of {MaxAttempts}. Retrying in {DelaySeconds} seconds",
                    migrationName,
                    attempt,
                    maxAttempts,
                    delay.TotalSeconds);

                await Task.Delay(delay, cancellationToken);
            }
        }

        using var finalScope = app.Services.CreateScope();
        var finalDbContext = finalScope.ServiceProvider.GetRequiredService<TDbContext>();
        await finalDbContext.Database.MigrateAsync(cancellationToken);
    }
}
