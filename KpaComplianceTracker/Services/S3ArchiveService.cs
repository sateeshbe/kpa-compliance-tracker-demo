
using System.Text;
using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KpaComplianceTracker.Services;

public sealed class S3Options
{
    public string Bucket { get; set; } = ""; // set via config (appsettings or env S3__Bucket)
}

public sealed class S3ArchiveService : IS3ArchiveService
{
    private readonly IAmazonS3 _s3;
    private readonly ILogger<S3ArchiveService> _logger;
    private readonly string _bucket;

    public S3ArchiveService(IAmazonS3 s3, IOptions<S3Options> opts, ILogger<S3ArchiveService> logger)
    {
        _s3 = s3;
        _logger = logger;
        _bucket = opts.Value.Bucket;
    }

    public async Task<string?> SavePayloadAsync(JsonElement payload, CancellationToken ct = default)
    {
        if (payload.ValueKind != JsonValueKind.Array || payload.GetArrayLength() == 0)
            return null;

        var s3Key = $"ingest/{DateTime.UtcNow:yyyy/MM/dd}/payload-{Guid.NewGuid()}.json";
        try
        {
            var raw = payload.GetRawText();
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(raw));
            await _s3.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _bucket,
                Key = s3Key,
                InputStream = ms,
                ContentType = "application/json"
            }, ct);

            _logger.LogInformation("Archived payload to s3://{Bucket}/{Key}", _bucket, s3Key);
            return s3Key;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to archive payload to S3 bucket {Bucket}", _bucket);
            return null; 
        }
    }
}
