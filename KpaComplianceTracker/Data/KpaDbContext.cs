using Microsoft.EntityFrameworkCore;
using KpaComplianceTracker.Entities;

namespace KpaComplianceTracker.Data;

public class KpaDbContext : DbContext
{
    public KpaDbContext(DbContextOptions<KpaDbContext> options) : base(options) { }
    public DbSet<ComplianceTask> Tasks => Set<ComplianceTask>();
}
