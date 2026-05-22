using Admission.Api.Auth;
using Admission.Api.Middlewares;
using Admission.Api.Options;
using Admission.Application.Extensions;
using Admission.DictionaryClient;
using Admission.FileStorage;
using Admission.Infrastructure;
using Admission.Messaging;
using Admission.Persistence;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAdmissionPersistence(builder.Configuration);
builder.Services.AddFileStorage(builder.Configuration);
builder.Services.AddAdmissionDictionaryClient(builder.Configuration);
builder.Services.AddAdmissionInfrastructure(builder.Configuration);
builder.Services.AddMessaging(builder.Configuration);
builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();

var admissionProgramsSection = builder.Configuration.GetSection(AdmissionProgramsOptions.SectionName);
var maxPrograms = int.TryParse(admissionProgramsSection[nameof(AdmissionProgramsOptions.MaxSelectedPrograms)], out var parsed)
    ? parsed : 5;
builder.Services.AddSingleton(new AdmissionProgramsOptions { MaxSelectedPrograms = maxPrograms });

var app = builder.Build();

await app.Services.ApplyAdmissionMigrationsAsync();
await app.Services.EnsureBucketExistsAsync();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
