using Admission.Application.Services;
using Admission.DictionaryClient.LocalDictionary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Admission.DictionaryClient;

public static class DependencyInjection
{
    public static IServiceCollection AddAdmissionDictionaryClient(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration["DictionaryApi:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("Dictionary API base url is missing. Configure 'DictionaryApi:BaseUrl'.");
        }

        services.AddSingleton(new DictionaryClientOptions
        {
            BaseUrl = baseUrl
        });
        services.AddHttpClient<ILocalDictionaryService, LocalDictionaryService>();

        return services;
    }
}
