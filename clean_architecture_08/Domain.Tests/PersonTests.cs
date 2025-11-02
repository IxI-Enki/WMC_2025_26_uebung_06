using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Xunit;

namespace Domain.Tests;

/// <summary>
/// Unit-Tests für Person-Entität und deren Validierungen.
/// </summary>
public class PersonTests
{
    private class FakePersonUniquenessChecker(bool unique) : IPersonUniquenessChecker
    {
        public Task<bool> IsUniqueAsync(int id, string mailAddress, CancellationToken ct = default) 
            => Task.FromResult(unique);
    }

    [Fact]
    public async Task CreateAsync_Succeeds_WithValidData()
    {
        // Arrange
        var checker = new FakePersonUniquenessChecker(true);

        // Act
        var person = await Person.CreateAsync("Müller", "Max", "max.mueller@example.com", checker);

        // Assert
        Assert.Equal("Müller", person.LastName);
        Assert.Equal("Max", person.FirstName);
        Assert.Equal("max.mueller@example.com", person.MailAddress);
    }

    [Theory]
    [InlineData(null, "Max", "max@test.com", "Nachname darf nicht leer sein.")]
    [InlineData("", "Max", "max@test.com", "Nachname darf nicht leer sein.")]
    [InlineData("Müller", "", "max@test.com", "Vorname darf nicht leer sein.")]
    [InlineData("Müller", "Max", "", "E-Mail-Adresse darf nicht leer sein.")]
    [InlineData("Müller", "Max", "invalid-email", "E-Mail-Adresse ist syntaktisch nicht korrekt.")]
    [InlineData("Müller", "Max", "no-at-sign", "E-Mail-Adresse ist syntaktisch nicht korrekt.")]
    public async Task CreateAsync_InvalidRules_Throws(string? lastName, string? firstName, string? mailAddress, string expectedMessage)
    {
        // Arrange
        var checker = new FakePersonUniquenessChecker(true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainValidationException>(() => 
            Person.CreateAsync(lastName ?? string.Empty, firstName ?? string.Empty, mailAddress ?? string.Empty, checker));
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_Throws()
    {
        // Arrange
        var checker = new FakePersonUniquenessChecker(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainValidationException>(() => 
            Person.CreateAsync("Müller", "Max", "max@test.com", checker));
        Assert.Equal("Eine Person mit dieser E-Mail-Adresse existiert bereits.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_ChangesValues_WhenUnique()
    {
        // Arrange
        var checker = new FakePersonUniquenessChecker(true);
        var person = await Person.CreateAsync("Müller", "Max", "max@test.com", checker);

        // Act
        await person.UpdateAsync("Schmidt", "Anna", "anna@test.com", checker);

        // Assert
        Assert.Equal("Schmidt", person.LastName);
        Assert.Equal("Anna", person.FirstName);
        Assert.Equal("anna@test.com", person.MailAddress);
    }

    [Fact]
    public async Task UpdateAsync_DuplicateEmail_Throws()
    {
        // Arrange
        var checkerUnique = new FakePersonUniquenessChecker(true);
        var person = await Person.CreateAsync("Müller", "Max", "max@test.com", checkerUnique);
        var checkerDuplicate = new FakePersonUniquenessChecker(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainValidationException>(() => 
            person.UpdateAsync("Müller", "Max", "other@test.com", checkerDuplicate));
        Assert.Equal("Eine Person mit dieser E-Mail-Adresse existiert bereits.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_SameValues_NoUniquenessCheckNeeded()
    {
        // Arrange
        bool uniquenessCalled = false;
        var checker = new CallbackUniquenessChecker(() => uniquenessCalled = true);
        var person = await Person.CreateAsync("Müller", "Max", "max@test.com", new FakePersonUniquenessChecker(true));

        // Act
        await person.UpdateAsync("Müller", "Max", "max@test.com", checker); // same values

        // Assert
        Assert.False(uniquenessCalled); // uniqueness check not needed because values unchanged
    }

    [Fact]
    public async Task ToString_ReturnsFormattedString()
    {
        // Arrange & Act
        var person = await Person.CreateAsync("Müller", "Max", "max@test.com", new FakePersonUniquenessChecker(true));
        var result = person.ToString();

        // Assert
        Assert.Equal("Max Müller (max@test.com)", result);
    }

    private class CallbackUniquenessChecker(Action callback) : IPersonUniquenessChecker
    {
        public Task<bool> IsUniqueAsync(int id, string mailAddress, CancellationToken ct = default)
        {
            callback();
            return Task.FromResult(true);
        }
    }
}

