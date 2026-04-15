using Admission.Dictionary.Abstractions;
using Admission.Dictionary.Client;
using Admission.Dictionary.Middlewares;
using Admission.Dictionary.Persistence;
using Admission.Dictionary.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterDictionaryClient();
builder.Services.AddDictionaryPersistence(builder.Configuration);

builder.Services.AddScoped<IDictionaryService, DictionaryService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<ILocalDictionaryService, LocalDictionaryService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.ApplyDictionaryMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.MapHealthChecks("/health");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();