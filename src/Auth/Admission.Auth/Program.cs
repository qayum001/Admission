using Admission.Auth.Api.Middleware;
using Admission.Auth.Composition;
using Admission.Auth.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthCore(builder.Configuration);

var app = builder.Build();

await app.ApplyAuthMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
