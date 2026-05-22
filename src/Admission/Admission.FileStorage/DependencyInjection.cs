using Admission.Application.Services;
using Admission.FileStorage.S3;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Admission.FileStorage;

public static class DependencyInjection
{
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var options = BuildOptions(configuration);

        services.AddSingleton(options);

        services.AddSingleton<IAmazonS3>(_ => BuildClient(options.ServiceUrl, options));

        var presignUrl = options.PublicServiceUrl ?? options.ServiceUrl;
        services.AddSingleton(new PresigningS3Client(BuildClient(presignUrl, options)));

        services.AddScoped<IFileStorage, S3FileStorage>();

        return services;
    }

    public static async Task EnsureBucketExistsAsync(this IServiceProvider services)
    {
        var s3 = services.GetRequiredService<IAmazonS3>();
        var options = services.GetRequiredService<S3FileStorageOptions>();

        var exists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(s3, options.BucketName);
        if (!exists)
            await s3.PutBucketAsync(new PutBucketRequest { BucketName = options.BucketName, UseClientRegion = true });
    }

    private static AmazonS3Client BuildClient(string serviceUrl, S3FileStorageOptions options)
    {
        var useHttp = options.UseHttp;
        if (Uri.TryCreate(serviceUrl, UriKind.Absolute, out var uri))
            useHttp = uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase);

        return new AmazonS3Client(
            new BasicAWSCredentials(options.AccessKey, options.SecretKey),
            new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = options.ForcePathStyle,
                UseHttp = useHttp
            });
    }

    private static S3FileStorageOptions BuildOptions(IConfiguration configuration)
    {
        var section = configuration.GetSection(S3FileStorageOptions.SectionName);

        var serviceUrl = section["ServiceUrl"]
            ?? section["Endpoint"]
            ?? configuration.GetConnectionString("S3")
            ?? string.Empty;
        var accessKey = section["AccessKey"]
            ?? configuration["MINIO_ROOT_USER"]
            ?? string.Empty;
        var secretKey = section["SecretKey"]
            ?? configuration["MINIO_ROOT_PASSWORD"]
            ?? string.Empty;
        var bucketName = section["BucketName"]
            ?? section["Bucket"]
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(serviceUrl))
            throw new InvalidOperationException(
                $"S3 service url is missing. Configure '{S3FileStorageOptions.SectionName}:ServiceUrl' or 'ConnectionStrings:S3'.");

        if (string.IsNullOrWhiteSpace(accessKey))
            throw new InvalidOperationException(
                $"S3 access key is missing. Configure '{S3FileStorageOptions.SectionName}:AccessKey'.");

        if (string.IsNullOrWhiteSpace(secretKey))
            throw new InvalidOperationException(
                $"S3 secret key is missing. Configure '{S3FileStorageOptions.SectionName}:SecretKey'.");

        if (string.IsNullOrWhiteSpace(bucketName))
            throw new InvalidOperationException(
                $"S3 bucket name is missing. Configure '{S3FileStorageOptions.SectionName}:BucketName'.");

        var useHttp = TryParseBoolean(section["UseHttp"]);
        if (!useHttp.HasValue && Uri.TryCreate(serviceUrl, UriKind.Absolute, out var endpointUri))
            useHttp = endpointUri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase);

        return new S3FileStorageOptions
        {
            ServiceUrl = serviceUrl,
            PublicServiceUrl = section["PublicServiceUrl"],
            AccessKey = accessKey,
            SecretKey = secretKey,
            BucketName = bucketName,
            ForcePathStyle = TryParseBoolean(section["ForcePathStyle"]) ?? true,
            UseHttp = useHttp ?? false
        };
    }

    private static bool? TryParseBoolean(string? rawValue) =>
        bool.TryParse(rawValue, out var parsed) ? parsed : null;
}
