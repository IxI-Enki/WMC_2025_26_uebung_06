using System.Net;
using System.Net.Http.Json;
using Application.Features.Devices.Commands.CreateDevice;
using Application.Features.Devices.Commands.UpdateDevice;
using Application.Features.Dtos;
using Api.Tests.Utilities;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Api.Tests.Devices;

/// <summary>
/// Integration tests for Devices endpoints.
/// </summary>
public class DevicesEndpointTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateDevice_Returns201_AndBody()
    {
        // Arrange
        var cmd = new CreateDeviceCommand("ABC123", "Samsung Galaxy", DeviceType.SmartPhone);

        // Act
        var response = await _client.PostAsJsonAsync("/api/devices", cmd);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<GetDeviceDto>();
        dto.Should().NotBeNull();
        dto!.SerialNumber.Should().Be("ABC123");
        dto.DeviceName.Should().Be("Samsung Galaxy");
        dto.DeviceType.Should().Be(DeviceType.SmartPhone);
    }

    [Fact]
    public async Task CreateDevice_InvalidSerialNumber_Returns400()
    {
        // Arrange - SerialNumber too short
        var cmd = new CreateDeviceCommand("AB", "Device", DeviceType.Tablet);

        // Act
        var response = await _client.PostAsJsonAsync("/api/devices", cmd);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsList()
    {
        // Arrange
        await _client.PostAsJsonAsync("/api/devices", new CreateDeviceCommand("DEV1", "Device 1", DeviceType.Tablet));
        await _client.PostAsJsonAsync("/api/devices", new CreateDeviceCommand("DEV2", "Device 2", DeviceType.Notebook));

        // Act
        var response = await _client.GetAsync("/api/devices");

        // Assert
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<GetDeviceDto>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/devices/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Found_Returns200()
    {
        // Arrange
        var create = new CreateDeviceCommand("GETID123", "GetById Device", DeviceType.SmartPhone);
        var createdResp = await _client.PostAsJsonAsync("/api/devices", create);
        var created = await createdResp.Content.ReadFromJsonAsync<GetDeviceDto>();

        // Act
        var response = await _client.GetAsync($"/api/devices/{created!.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<GetDeviceDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(created.Id);
        dto.SerialNumber.Should().Be("GETID123");
    }

    [Fact]
    public async Task GetAllWithCounts_ReturnsListWithCounts()
    {
        // Arrange
        var device = new CreateDeviceCommand("COUNT1", "Count Device", DeviceType.Tablet);
        await _client.PostAsJsonAsync("/api/devices", device);

        // Act
        var response = await _client.GetAsync("/api/devices/with-counts");

        // Assert
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<GetDeviceWithCountDto>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterOrEqualTo(1);
        list.Should().OnlyContain(d => d.NumberOfUsages >= 0);
    }

    [Fact]
    public async Task UpdateDevice_Works()
    {
        // Arrange
        var create = new CreateDeviceCommand("UPDATE1", "Old Name", DeviceType.Tablet);
        var createdResp = await _client.PostAsJsonAsync("/api/devices", create);
        var created = await createdResp.Content.ReadFromJsonAsync<GetDeviceDto>();

        var updateCmd = new UpdateDeviceCommand(created!.Id, "UPDATE1", "New Name", DeviceType.SmartPhone);

        // Act
        var updateResp = await _client.PutAsJsonAsync($"/api/devices/{created.Id}", updateCmd);

        // Assert
        updateResp.EnsureSuccessStatusCode();
        var updated = await updateResp.Content.ReadFromJsonAsync<GetDeviceDto>();
        updated!.DeviceName.Should().Be("New Name");
        updated.DeviceType.Should().Be(DeviceType.SmartPhone);
    }

    [Fact]
    public async Task UpdateDevice_IdMismatch_Returns400()
    {
        // Arrange
        var create = new CreateDeviceCommand("MISMATCH1", "Device", DeviceType.Tablet);
        var createdResp = await _client.PostAsJsonAsync("/api/devices", create);
        var created = await createdResp.Content.ReadFromJsonAsync<GetDeviceDto>();

        var updateCmd = new UpdateDeviceCommand(created!.Id + 999, "MISMATCH1", "Device", DeviceType.Tablet);

        // Act
        var updateResp = await _client.PutAsJsonAsync($"/api/devices/{created.Id}", updateCmd);

        // Assert
        updateResp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteDevice_Works()
    {
        // Arrange
        var create = new CreateDeviceCommand("DELETE1", "Delete Device", DeviceType.Notebook);
        var createdResp = await _client.PostAsJsonAsync("/api/devices", create);
        var created = await createdResp.Content.ReadFromJsonAsync<GetDeviceDto>();

        // Act
        var delResp = await _client.DeleteAsync($"/api/devices/{created!.Id}");

        // Assert
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResp = await _client.GetAsync($"/api/devices/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

