using Admission.MailManager.Inbox.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Admission.MailManager.Inbox.Persistence;

public sealed class EfCoreInboxMessageWriter(InboxDbContext dbContext) : IInboxMessageWriter
{
    public async Task<InboxStoreResult> StoreAsync(InboxMessage message, CancellationToken cancellationToken = default)
    {
        dbContext.InboxMessages.Add(message);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return InboxStoreResult.Inserted;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            dbContext.Entry(message).State = EntityState.Detached;
            return InboxStoreResult.Duplicate;
        }
    }
}
