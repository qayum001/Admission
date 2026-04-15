using Admission.MailManager.Inbox.Persistence;
using Admission.MailManager.Inbox.Processing;
using Admission.MailManager.Messaging;
using Admission.MailManager.Mailing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddInboxPersistence(builder.Configuration);
builder.Services.RegisterSmtp(builder.Configuration);
builder.Services.AddInboxProcessing(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

await app.ApplyInboxMigrationsAsync();

app.MapHealthChecks("/health");

app.Run();
// alba jasperfx
// reqnroll 
// BDD
