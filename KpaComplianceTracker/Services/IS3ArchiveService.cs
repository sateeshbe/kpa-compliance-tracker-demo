// Services/IS3ArchiveService.cs
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KpaComplianceTracker.Services;

public interface IS3ArchiveService
{
    /// <summary>Uploads the raw payload to S3 and returns the S3 key (or null on failure).</summary>
    Task<string?> SavePayloadAsync(JsonElement payload, CancellationToken ct = default);
}
