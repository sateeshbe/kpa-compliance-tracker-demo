using System.Text.Json.Serialization;

namespace KpaComplianceTracker.Contracts;

public sealed class ComplianceTaskUpsertDto
{
    public Guid? Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("site")]
    public string? Site { get; set; }

    [JsonPropertyName("owner")]
    public string? Owner { get; set; }

    [JsonPropertyName("dueDate")]
    public string? DueDate { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
