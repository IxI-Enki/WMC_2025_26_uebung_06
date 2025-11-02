using System.Net;
using System.Net.Http.Json;
using Application.Features.Usages.Commands.CreateUsage;
using Application.Features.Usages.Commands.UpdateUsage;
using Application.Features.Devices.Commands.CreateDevice;
using Application.Features.People.Commands.CreatePerson;
using Application.Features.Dtos;
using Api.Tests.Utilities;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Api.Tests.Usages;

/// <summary>
/// Integration tests for Usages endpoints.
/// </summary>
public class UsagesEndpointTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<(int deviceId, int personId)> CreateDeviceAndPerson(string deviceSerial, string personEmail)
    {
        var deviceCmd = new CreateDeviceCommand(deviceSerial, "Test Device", DeviceType.Tablet);
        var deviceResp = await _client.PostAsJsonAsync("/api/devices", deviceCmd);
        deviceResp.EnsureSuccessStatusCode(); // Ensure device was created successfully
        var device = await deviceResp.Content.ReadFromJsonAsync<GetDeviceDto>();

        var personCmd = new CreatePersonCommand("Test", "User", personEmail);
        var personResp = await _client.PostAsJsonAsync("/api/people", personCmd);
        personResp.EnsureSuccessStatusCode(); // Ensure person was created successfully
        var person = await personResp.Content.ReadFromJsonAsync<GetPersonDto>();

        return (device!.Id, person!.Id);
    }

    [Fact]
    public async Task CreateUsage_Returns201_AndBody()
    {
        // Arrange
        var (deviceId, personId) = await CreateDeviceAndPerson("USAGE1", "usage1@test.com");
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        var cmd = new CreateUsageCommand(deviceId, personId, tomorrow, nextWeek);

        // Act
        var response = await _client.PostAsJsonAsync("/api/usages", cmd);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<GetUsageDto>();
        dto.Should().NotBeNull();
        dto!.DeviceId.Should().Be(deviceId);
        dto.PersonId.Should().Be(personId);
        dto.From.Should().Be(tomorrow);
        dto.To.Should().Be(nextWeek);
    }

    [Fact]
    public async Task CreateUsage_PastDate_Returns400()
    {
        // Arrange
        var (deviceId, personId) = await CreateDeviceAndPerson("PAST1", "past@test.com");
        var yesterday = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var cmd = new CreateUsageCommand(deviceId, personId, yesterday, tomorrow);

        // Act
        var response = await _client.PostAsJsonAsync("/api/usages", cmd);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUsage_ToBeforeFrom_Returns400()
    {
        // Arrange
        var (deviceId, personId) = await CreateDeviceAndPerson("INVALID1", "invalid@test.com");
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var yesterday = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var cmd = new CreateUsageCommand(deviceId, personId, tomorrow, yesterday);

        // Act
        var response = await _client.PostAsJsonAsync("/api/usages", cmd);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUsage_Overlap_Returns400()
    {
        // Arrange
        var (deviceId, personId) = await CreateDeviceAndPerson("OVERLAP1", "overlap@test.com");
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));

        // Create first usage
        var cmd1 = new CreateUsageCommand(deviceId, personId, tomorrow, nextWeek);
        await _client.PostAsJsonAsync("/api/usages", cmd1);

        // Try to create overlapping usage
        var dayAfterTomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        var dayAfterNextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(8));
        var cmd2 = new CreateUsageCommand(deviceId, personId, dayAfterTomorrow, dayAfterNextWeek);

        // Act
        var response = await _client.PostAsJsonAsync("/api/usages", cmd2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsList()
    {
        // Arrange
        var (deviceId1, personId1) = await CreateDeviceAndPerson("GETALL1", "getall1@test.com");
        var (deviceId2, personId2) = await CreateDeviceAndPerson("GETALL2", "getall2@test.com");
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));

        await _client.PostAsJsonAsync("/api/usages", new CreateUsageCommand(deviceId1, personId1, tomorrow, nextWeek));
        await _client.PostAsJsonAsync("/api/usages", new CreateUsageCommand(deviceId2, personId2, tomorrow, nextWeek));

        // Act
        var response = await _client.GetAsync("/api/usages");

        // Assert
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<GetUsageDto>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/usages/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Found_Returns200()
    {
        // Arrange
        var (deviceId, personId) = await CreateDeviceAndPerson("GETID1", "getid@test.com");
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        var cmd = new CreateUsageCommand(deviceId, personId, tomorrow, nextWeek);
        var createdResp = await _client.PostAsJsonAsync("/api/usages", cmd);
        var created = await createdResp.Content.ReadFromJsonAsync<GetUsageDto>();

        // Act
        var response = await _client.GetAsync($"/api/usages/{created!.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<GetUsageDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task UpdateUsage_Works()
    {
        // Arrange
        var (deviceId, personId) = await CreateDeviceAndPerson("UPDATE1", "update@test.com");
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        var cmd = new CreateUsageCommand(deviceId, personId, tomorrow, nextWeek);
        var createdResp = await _client.PostAsJsonAsync("/api/usages", cmd);
        var created = await createdResp.Content.ReadFromJsonAsync<GetUsageDto>();

        var twoWeeks = DateOnly.FromDateTime(DateTime.Today.AddDays(14));
        var updateCmd = new UpdateUsageCommand(created!.Id, deviceId, personId, nextWeek, twoWeeks);

        // Act
        var updateResp = await _client.PutAsJsonAsync($"/api/usages/{created.Id}", updateCmd);

        // Assert
        updateResp.EnsureSuccessStatusCode();
        var updated = await updateResp.Content.ReadFromJsonAsync<GetUsageDto>();
        updated!.From.Should().Be(nextWeek);
        updated.To.Should().Be(twoWeeks);
    }

    [Fact]
    public async Task DeleteUsage_Works()
    {
        // Arrange - use unique identifiers to avoid conflicts with other tests
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var (deviceId, personId) = await CreateDeviceAndPerson($"DEL{uniqueId}", $"delete{uniqueId}@test.com");
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        var cmd = new CreateUsageCommand(deviceId, personId, tomorrow, nextWeek);
        var createdResp = await _client.PostAsJsonAsync("/api/usages", cmd);
        var created = await createdResp.Content.ReadFromJsonAsync<GetUsageDto>();

        // Act
        var delResp = await _client.DeleteAsync($"/api/usages/{created!.Id}");

        // Assert
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResp = await _client.GetAsync($"/api/usages/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

