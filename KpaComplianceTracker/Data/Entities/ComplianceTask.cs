using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KpaComplianceTracker.Entities;

public class ComplianceTask
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(256)]
    public string Title { get; set; } = default!;

    [MaxLength(100)] 
    public string? Category { get; set; }
    [MaxLength(100)] 
    public string? Site { get; set; }
    [MaxLength(100)] 
    public string? Owner { get; set; }

    [Column(TypeName = "date")]
    public DateOnly? DueDate { get; set; }

    [MaxLength(50)] 
    public string? Status { get; set; }
    [MaxLength(512)] 
    public string? SourceS3Key { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
