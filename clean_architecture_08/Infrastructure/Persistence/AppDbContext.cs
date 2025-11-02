using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext. Verwaltet die Verbindung zur Datenbank und das Mapping der Entitäten.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Tabelle/DbSet für Devices.
    /// </summary>
    public DbSet<Device> Devices => Set<Device>();

    /// <summary>
    /// Tabelle/DbSet für People.
    /// </summary>
    public DbSet<Person> People => Set<Person>();

    /// <summary>
    /// Tabelle/DbSet für Usages.
    /// </summary>
    public DbSet<Usage> Usages => Set<Usage>();

    /// <summary>
    /// Fluent-API Konfigurationen für EF Core.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Device Configuration
        modelBuilder.Entity<Device>(device =>
        {
            device.Property(d => d.SerialNumber).HasMaxLength(100).IsRequired();
            device.Property(d => d.DeviceName).HasMaxLength(200).IsRequired();
            device.Property(d => d.DeviceType).IsRequired();

            // RowVersion für Optimistic Concurrency
            device.Property(d => d.RowVersion).IsRowVersion();

            // Uniqueness constraint auf SerialNumber
            device.HasIndex(d => d.SerialNumber)
                  .IsUnique()
                  .HasDatabaseName("UX_Devices_SerialNumber");
        });

        // Person Configuration
        modelBuilder.Entity<Person>(person =>
        {
            person.Property(p => p.LastName).HasMaxLength(100).IsRequired();
            person.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
            person.Property(p => p.MailAddress).HasMaxLength(255).IsRequired();

            // RowVersion für Optimistic Concurrency
            person.Property(p => p.RowVersion).IsRowVersion();

            // Uniqueness constraint auf MailAddress
            person.HasIndex(p => p.MailAddress)
                  .IsUnique()
                  .HasDatabaseName("UX_People_MailAddress");
        });

        // Usage Configuration
        modelBuilder.Entity<Usage>(usage =>
        {
            // RowVersion für Optimistic Concurrency
            usage.Property(u => u.RowVersion).IsRowVersion();

            // Index für häufige Abfragen nach Device und Datumsbereich
            usage.HasIndex(u => new { u.DeviceId, u.From, u.To })
                 .HasDatabaseName("IX_Usages_Device_DateRange");

            // Beziehung: Jede Usage gehört zu genau einem Device (Cascade Delete)
            usage.HasOne(u => u.Device)
                .WithMany(d => d.Usages)
                .HasForeignKey(u => u.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Beziehung: Jede Usage gehört zu genau einer Person (Cascade Delete)
            usage.HasOne(u => u.Person)
                .WithMany(p => p.Usages)
                .HasForeignKey(u => u.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
