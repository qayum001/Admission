using Amazon.S3;

namespace Admission.FileStorage.S3;

internal sealed class PresigningS3Client(IAmazonS3 inner)
{
    public IAmazonS3 Client { get; } = inner;
}
