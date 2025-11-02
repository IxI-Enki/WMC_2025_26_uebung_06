using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using System.Reflection;
using Xunit;

namespace Domain.Tests;

/// <summary>
/// Unit-Tests für Usage-Entität und deren Validierungen.
/// </summary>
public class UsageTests
{
    private class FakeOverlapChecker(bool hasOverlap) : IUsageOverlapChecker
    {
        public Task<bool> HasOverlapAsync(int id, int deviceId, DateOnly from, DateOnly to, CancellationToken ct = default)
            => Task.FromResult(hasOverlap);
    }

    private class FakePersonUniquenessChecker(bool unique) : IPersonUniquenessChecker
    {
        public Task<bool> IsUniqueAsync(int id, string mailAddress, CancellationToken ct = default)
            => Task.FromResult(unique);
    }

    /// <summary>
    /// Helper method to set the Id property of an entity using reflection (for testing purposes)
    /// </summary>
    private static void SetEntityId(BaseEntity entity, int id)
    {
        var idProperty = typeof(BaseEntity).GetProperty("Id");
        idProperty!.SetValue(entity, id);
    }

    [Fact]
    public async Task CreateAsync_Succeeds_WithValidData()
    {
        // Arrange
        var device = Device.Create("123", "Test Device", DeviceType.Tablet);
        SetEntityId(device, 1);
        var person = await Person.CreateAsync("Müller", "Max", "max@test.com", new FakePersonUniquenessChecker(true));
        SetEntityId(person, 2);
        var overlapChecker = new FakeOverlapChecker(false);
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));

        // Act
        var usage = await Usage.CreateAsync(device, person, tomorrow, nextWeek, overlapChecker);

        // Assert
        Assert.Equal(device.Id, usage.DeviceId);
        Assert.Equal(person.Id, usage.PersonId);
        Assert.Equal(tomorrow, usage.From);
        Assert.Equal(nextWeek, usage.To);
    }

    [Fact]
    public async Task CreateAsync_NullDevice_Throws()
    {
        // Arrange
        var person = await Person.CreateAsync("Müller", "Max", "max@test.com", new FakePersonUniquenessChecker(true));
        SetEntityId(person, 1);
        var overlapChecker = new FakeOverlapChecker(false);
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            Usage.CreateAsync(null!, person, tomorrow, nextWeek, overlapChecker));
    }

    [Fact]
    public async Task CreateAsync_NullPerson_Throws()
    {
        // Arrange
        var device = Device.Create("123", "Test Device", DeviceType.Tablet);
        SetEntityId(device, 1);
        var overlapChecker = new FakeOverlapChecker(false);
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            Usage.CreateAsync(device, null!, tomorrow, nextWeek, overlapChecker));
    }

    [Fact]
    public async Task CreateAsync_ToBeforeFrom_Throws()
    {
        // Arrange
        var device = Device.Create("123", "Test Device", DeviceType.Tablet);
        SetEntityId(device, 1);
        var person = await Person.CreateAsync("Müller", "Max", "max@test.com", new FakePersonUniquenessChecker(true));
        SetEntityId(person, 2);
        var overlapChecker = new FakeOverlapChecker(false);
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var yesterday = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            Usage.CreateAsync(device, person, tomorrow, yesterday, overlapChecker));
        Assert.Equal("Das Rückgabedatum darf nicht vor dem Startdatum liegen.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_PastDate_Throws()
    {
        // Arrange
        var device = Device.Create("123", "Test Device", DeviceType.Tablet);
        SetEntityId(device, 1);
        var person = await Person.CreateAsync("Müller", "Max", "max@test.com", new FakePersonUniquenessChecker(true));
        SetEntityId(person, 2);
        var overlapChecker = new FakeOverlapChecker(false);
        var yesterday = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            Usage.CreateAsync(device, person, yesterday, tomorrow, overlapChecker, allowPastDates: false));
        Assert.Equal("Nutzungen können nur für die Zukunft gebucht werden.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_PastDate_AllowedForImport()
    {
        // Arrange
        var device = Device.Create("123", "Test Device", DeviceType.Tablet);
        SetEntityId(device, 1);
        var person = await Person.CreateAsync("Müller", "Max", "max@test.com", new FakePersonUniquenessChecker(true));
        SetEntityId(person, 2);
        var overlapChecker = new FakeOverlapChecker(false);
        var lastWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
        var yesterday = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act
        var usage = await Usage.CreateAsync(device, person, lastWeek, yesterday, overlapChecker, allowPastDates: true);

        // Assert
        Assert.Equal(lastWeek, usage.From);
        Assert.Equal(yesterday, usage.To);
    }

    [Fact]
    public async Task CreateAsync_OverlappingUsage_Throws()
    {
        // Arrange
        var device = Device.Create("123", "Test Device", DeviceType.Tablet);
        SetEntityId(device, 1);
        var person = await Person.CreateAsync("Müller", "Max", "max@test.com", new FakePersonUniquenessChecker(true));
        SetEntityId(person, 2);
        var overlapChecker = new FakeOverlapChecker(true); // Overlap detected
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            Usage.CreateAsync(device, person, tomorrow, nextWeek, overlapChecker));
        Assert.Equal("Die Nutzung überlappt mit einer anderen Buchung für dieses Gerät.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_ChangesValues_WhenNoOverlap()
    {
        // Arrange
        var device1 = Device.Create("123", "Device 1", DeviceType.Tablet);
        SetEntityId(device1, 1);
        var device2 = Device.Create("456", "Device 2", DeviceType.SmartPhone);
        SetEntityId(device2, 2);
        var person1 = await Person.CreateAsync("Müller", "Max", "max@test.com", new FakePersonUniquenessChecker(true));
        SetEntityId(person1, 3);
        var person2 = await Person.CreateAsync("Schmidt", "Anna", "anna@test.com", new FakePersonUniquenessChecker(true));
        SetEntityId(person2, 4);
        var overlapChecker = new FakeOverlapChecker(false);
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        var nextMonth = DateOnly.FromDateTime(DateTime.Today.AddDays(30));

        var usage = await Usage.CreateAsync(device1, person1, tomorrow, nextWeek, overlapChecker);

        // Act
        await usage.UpdateAsync(device2, person2, nextWeek, nextMonth, overlapChecker);

        // Assert
        Assert.Equal(device2.Id, usage.DeviceId);
        Assert.Equal(person2.Id, usage.PersonId);
        Assert.Equal(nextWeek, usage.From);
        Assert.Equal(nextMonth, usage.To);
    }

    [Fact]
    public async Task UpdateAsync_SameValues_NoChange()
    {
        // Arrange
        bool overlapCalled = false;
        var overlapChecker = new CallbackOverlapChecker(() => overlapCalled = true);
        var device = Device.Create("123", "Device", DeviceType.Tablet);
        SetEntityId(device, 1);
        var person = await Person.CreateAsync("Müller", "Max", "max@test.com", new FakePersonUniquenessChecker(true));
        SetEntityId(person, 2);
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));

        var usage = await Usage.CreateAsync(device, person, tomorrow, nextWeek, new FakeOverlapChecker(false));

        // Act
        await usage.UpdateAsync(device, person, tomorrow, nextWeek, overlapChecker); // same values

        // Assert
        Assert.False(overlapCalled); // overlap check not needed because values unchanged
    }

    private class CallbackOverlapChecker(Action callback) : IUsageOverlapChecker
    {
        public Task<bool> HasOverlapAsync(int id, int deviceId, DateOnly from, DateOnly to, CancellationToken ct = default)
        {
            callback();
            return Task.FromResult(false);
        }
    }
}

