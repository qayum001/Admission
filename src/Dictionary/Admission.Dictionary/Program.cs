using System.Net.Http.Headers;
using System.Text;
using Admission.Dictionary.Abstractions;
using Admission.Dictionary.Middlewares;
using Admission.Dictionary.Services;
using Dictionary.Integration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<DictionaryClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    client.BaseAddress = new Uri(config["DictionaryApi:BaseUrl"]!);

    var rawCreds = $"{config["DictionaryApi:Login"]}:{config["DictionaryApi:Password"]}";
    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawCreds));

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);
});

builder.Services.AddScoped<IDictionaryService, DictionaryService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();