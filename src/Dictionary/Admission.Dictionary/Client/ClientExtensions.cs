using System.Net.Http.Headers;
using System.Text;
using Dictionary.Integration;

namespace Admission.Dictionary.Client;

public static class ClientExtensions
{
    public static void RegisterDictionaryClient(this IServiceCollection services)
    {
        services.AddHttpClient<DictionaryClient>((sp, client) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            client.BaseAddress = new Uri(config["DictionaryApi:BaseUrl"]!);

            var rawCreds = $"{config["DictionaryApi:Login"]}:{config["DictionaryApi:Password"]}";
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawCreds));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);
        });
    }
}