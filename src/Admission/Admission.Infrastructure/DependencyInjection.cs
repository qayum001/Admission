using Admission.Application.Services;
using Admission.Infrastructure.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Admission.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAdmissionInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var fileValidationOptions = BuildFileValidationOptions(configuration);
        var fileKeyOptions = BuildFileKeyOptions(configuration);

        services.AddSingleton(fileValidationOptions);
        services.AddSingleton(fileKeyOptions);

        services.AddSingleton<IFileValidationService, FileValidationService>();
        services.AddSingleton<IFileKeyGenerator, FileKeyGenerator>();

        return services;
    }

    private static FileValidationOptions BuildFileValidationOptions(IConfiguration configuration)
    {
        var section = configuration.GetSection(FileValidationOptions.SectionName);
        var rawMaxSize = section[nameof(FileValidationOptions.MaxSizeBytes)];
        var maxSize = long.TryParse(rawMaxSize, out var parsedMaxSize)
            ? parsedMaxSize
            : 10 * 1024 * 1024L;

        var allowedContentTypes = section
            .GetSection(nameof(FileValidationOptions.AllowedContentTypes))
            .GetChildren()
            .Select(x => x.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>()
            .ToArray();

        return new FileValidationOptions
        {
            MaxSizeBytes = maxSize,
            AllowedContentTypes = allowedContentTypes.Length > 0
                ? allowedContentTypes
                : ["application/pdf", "image/jpeg", "image/png"]
        };
    }

    private static FileKeyOptions BuildFileKeyOptions(IConfiguration configuration)
    {
        var rootPrefix = configuration[$"{FileKeyOptions.SectionName}:RootPrefix"];

        return new FileKeyOptions
        {
            RootPrefix = string.IsNullOrWhiteSpace(rootPrefix) ? "admission" : rootPrefix
        };
    }
}
