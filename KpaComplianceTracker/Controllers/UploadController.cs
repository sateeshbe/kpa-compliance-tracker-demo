using KpaComplianceTracker.Contracts;
using KpaComplianceTracker.Data;
using KpaComplianceTracker.Entities;
using KpaComplianceTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;

namespace KpaComplianceTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly KpaDbContext _db;
    private readonly IS3ArchiveService _s3Archive;
    private readonly ILogger<UploadController> _logger;

    public UploadController(KpaDbContext db, IS3ArchiveService s3Archive, ILogger<UploadController> logger)
    {
        _db = db;
        _s3Archive = s3Archive;
        _logger = logger;
    }
    [HttpPost]
    public async Task<IActionResult> Upload([FromBody] List<ComplianceTaskUpsertDto> payload, CancellationToken ct)
    {
        _logger.LogInformation("Received upload request with {Count} items at {Time}", payload?.Count ?? 0, DateTime.UtcNow);

        if (payload is null || payload.Count == 0)
        {
            _logger.LogWarning("Upload failed: payload is null or empty");
            return BadRequest("Empty payload");
        }

        // Archive the the received JSON
        var s3Key = await _s3Archive.SavePayloadAsync(JsonSerializer.SerializeToElement(payload), ct);

        int inserted = 0, updated = 0;

        foreach (var dto in payload)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Title))
                {
                    _logger.LogWarning("Skipping row: missing title");
                    continue;
                }

                var id = dto.Id ?? Guid.NewGuid();
                var existing = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);

                if (existing is null)
                {
                    _db.Tasks.Add(new ComplianceTask
                    {
                        Id = id,
                        Title = dto.Title.Trim(),
                        Category = dto.Category,
                        Site = dto.Site,
                        Owner = dto.Owner,
                        DueDate = ToDateOnly(dto.DueDate),
                        Status = dto.Status,
                        SourceS3Key = s3Key,
                        UpdatedAt = DateTimeOffset.UtcNow
                    });
                    inserted++;
                }
                else
                {
                    existing.Title = dto.Title.Trim();
                    existing.Category = dto.Category;
                    existing.Site = dto.Site;
                    existing.Owner = dto.Owner;
                    existing.DueDate = ToDateOnly(dto.DueDate);
                    existing.Status = dto.Status;
                    existing.UpdatedAt = DateTimeOffset.UtcNow;
                    updated++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process one record in payload");
            }
        }

        try
        {
            var saved = await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Database changes saved: {Count} rows affected", saved);
            return Ok(new { inserted, updated, s3Key });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database save failed");
            return StatusCode(500, "An error occurred while saving to the database");
        }
    }

    private static DateOnly? ToDateOnly(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;

        if (DateOnly.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                   DateTimeStyles.None, out var d)) return d;

        if (DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture,
                                    DateTimeStyles.RoundtripKind, out var dto))
            return DateOnly.FromDateTime(dto.DateTime);

        throw new JsonException($"Invalid dueDate: '{s}'");
    }


}
