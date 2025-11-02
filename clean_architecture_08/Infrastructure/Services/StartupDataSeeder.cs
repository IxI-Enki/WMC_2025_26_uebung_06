using Application.Contracts;
using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Infrastructure.Services;

public sealed class StartupDataSeederOptions
{
    public string CsvPath { get; set; } = string.Empty;
}

/// <summary>
/// Hosted Service, der beim Start Migrationen ausführt und die DB einmalig aus CSV befüllt.
/// </summary>
public class StartupDataSeeder(IOptions<StartupDataSeederOptions> options, IServiceProvider serviceProvider) : IHostedService
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly string _csvPath = options.Value.CsvPath;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Database.CanConnect())
        {
            // Nur seeden, wenn noch keine Devices existieren (idempotent)
            if (await dbContext.Devices.AnyAsync(cancellationToken)) return;
            await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        }

        await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var personChecker = scope.ServiceProvider.GetRequiredService<IPersonUniquenessChecker>();
        var overlapChecker = scope.ServiceProvider.GetRequiredService<IUsageOverlapChecker>();

        var allUsages = await ReadUsagesFromCsv(uow, personChecker, overlapChecker, cancellationToken);
        dbContext.Usages.AddRange(allUsages);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Liest die CSV nur einmalig (Thread-sicher) und baut Device-, Person- und Usage-Objekte.
    /// CSV Format: SerialNumber; DeviceName; DeviceType; PersonLastName; PersonFirstName; MailAddress; From; To
    /// </summary>
    private async Task<IEnumerable<Usage>> ReadUsagesFromCsv(IUnitOfWork uow,
        IPersonUniquenessChecker personChecker, IUsageOverlapChecker overlapChecker, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(_csvPath))
            {
                throw new Exception($"File {_csvPath} doesn't exist");
            }

            var lines = await File.ReadAllLinesAsync(_csvPath, cancellationToken);
            var devices = new Dictionary<string, Device>();
            var people = new Dictionary<string, Person>();
            var usages = new List<Usage>(lines.Length);

            for (int i = 1; i < lines.Length; i++) // i=1: Header überspringen
            {
                var line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Parse CSV line: SerialNumber; DeviceName; DeviceType; PersonLastName; PersonFirstName; MailAddress; From; To
                var parts = line.Split(';');
                if (parts.Length < 8) continue;

                var serialNumber = parts[0].Trim();
                var deviceName = parts[1].Trim().Trim('"'); // Remove quotes
                var deviceTypeStr = parts[2].Trim();
                var personLastName = parts[3].Trim();
                var personFirstName = parts[4].Trim();
                var mailAddress = parts[5].Trim();
                var fromStr = parts[6].Trim();
                var toStr = parts[7].Trim();

                // Parse DeviceType
                if (!Enum.TryParse<DeviceType>(deviceTypeStr, true, out var deviceType))
                    continue;

                // Parse dates (format: dd.MM.yyyy)
                if (!DateOnly.TryParseExact(fromStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var from))
                    continue;
                if (!DateOnly.TryParseExact(toStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var to))
                    continue;

                // Get or create Device
                if (!devices.TryGetValue(serialNumber, out var device))
                {
                    device = Device.Create(serialNumber, deviceName, deviceType);
                    devices[serialNumber] = device;
                    await uow.Devices.AddAsync(device, cancellationToken);
                    await uow.SaveChangesAsync(cancellationToken); // Device muss gespeichert werden, damit Id gesetzt wird
                }

                // Get or create Person
                if (!people.TryGetValue(mailAddress, out var person))
                {
                    person = await Person.CreateAsync(personLastName, personFirstName, mailAddress,
                        personChecker, cancellationToken);
                    people[mailAddress] = person;
                    await uow.People.AddAsync(person, cancellationToken);
                    await uow.SaveChangesAsync(cancellationToken); // Person muss gespeichert werden, damit Id gesetzt wird
                }

                // Create Usage (allowPastDates = true for import)
                var usage = await Usage.CreateAsync(device, person, from, to,
                    overlapChecker, allowPastDates: true, cancellationToken);
                usages.Add(usage);
            }

            return usages;
        }
        finally
        {
            _lock.Release();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
