using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourOperatorDataImport.Core.Entities;

namespace TourOperatorDataImport.Infrastructure.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<PricingRecord> PricingRecords => Set<PricingRecord>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(ConfigureUser);
        modelBuilder.Entity<PricingRecord>(ConfigurePricingRecord);
    }

    private void ConfigureUser(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(e => e.PasswordHash)
            .IsRequired();
        builder.Property(e => e.Role)
            .IsRequired()
            .HasConversion<string>();
        builder.HasIndex(e => e.Username).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();
    }

    private void ConfigurePricingRecord(EntityTypeBuilder<PricingRecord> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RouteCode)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.SeasonCode)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.Date)
            .IsRequired();
        builder.Property(e => e.EconomyPrice)
            .HasPrecision(18, 2); // Alternative to HasColumnType
        builder.Property(e => e.BusinessPrice)
            .HasPrecision(18, 2); // Alternative to HasColumnType
        builder.HasIndex(e => new { e.TourOperatorId, e.Date, e.RouteCode });
        builder.HasIndex(e => e.TourOperatorId);
    }
}