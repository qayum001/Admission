using Admission.MailManager.Inbox.Models;
using Microsoft.EntityFrameworkCore;

namespace Admission.MailManager.Inbox.Persistence;

public sealed class InboxDbContext(DbContextOptions<InboxDbContext> options) : DbContext(options)
{
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InboxDbContext).Assembly);
    }
}
