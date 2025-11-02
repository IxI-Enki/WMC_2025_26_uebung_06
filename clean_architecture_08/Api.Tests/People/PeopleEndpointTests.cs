using System.Net;
using System.Net.Http.Json;
using Application.Features.People.Commands.CreatePerson;
using Application.Features.People.Commands.UpdatePerson;
using Application.Features.Dtos;
using Api.Tests.Utilities;
using FluentAssertions;
using Xunit;

namespace Api.Tests.People;

/// <summary>
/// Integration tests for People endpoints.
/// </summary>
public class PeopleEndpointTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreatePerson_Returns201_AndBody()
    {
        // Arrange
        var cmd = new CreatePersonCommand("Müller", "Max", "max.mueller@test.com");

        // Act
        var response = await _client.PostAsJsonAsync("/api/people", cmd);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<GetPersonDto>();
        dto.Should().NotBeNull();
        dto!.LastName.Should().Be("Müller");
        dto.FirstName.Should().Be("Max");
        dto.MailAddress.Should().Be("max.mueller@test.com");
    }

    [Fact]
    public async Task CreatePerson_DuplicateEmail_Returns400()
    {
        // Arrange
        var cmd = new CreatePersonCommand("Schmidt", "Anna", "duplicate@test.com");
        await _client.PostAsJsonAsync("/api/people", cmd); // first time

        // Act
        var response = await _client.PostAsJsonAsync("/api/people", cmd); // duplicate

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePerson_InvalidEmail_Returns400()
    {
        // Arrange - invalid email format
        var cmd = new CreatePersonCommand("Test", "User", "invalid-email");

        // Act
        var response = await _client.PostAsJsonAsync("/api/people", cmd);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsList()
    {
        // Arrange
        await _client.PostAsJsonAsync("/api/people", new CreatePersonCommand("Person1", "First", "person1@test.com"));
        await _client.PostAsJsonAsync("/api/people", new CreatePersonCommand("Person2", "Second", "person2@test.com"));

        // Act
        var response = await _client.GetAsync("/api/people");

        // Assert
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<GetPersonDto>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/people/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Found_Returns200()
    {
        // Arrange
        var create = new CreatePersonCommand("GetById", "Test", "getbyid@test.com");
        var createdResp = await _client.PostAsJsonAsync("/api/people", create);
        var created = await createdResp.Content.ReadFromJsonAsync<GetPersonDto>();

        // Act
        var response = await _client.GetAsync($"/api/people/{created!.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<GetPersonDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(created.Id);
        dto.MailAddress.Should().Be("getbyid@test.com");
    }

    [Fact]
    public async Task UpdatePerson_Works()
    {
        // Arrange
        var create = new CreatePersonCommand("Old", "Name", "update@test.com");
        var createdResp = await _client.PostAsJsonAsync("/api/people", create);
        var created = await createdResp.Content.ReadFromJsonAsync<GetPersonDto>();

        var updateCmd = new UpdatePersonCommand(created!.Id, "New", "Name", "newemail@test.com");

        // Act
        var updateResp = await _client.PutAsJsonAsync($"/api/people/{created.Id}", updateCmd);

        // Assert
        updateResp.EnsureSuccessStatusCode();
        var updated = await updateResp.Content.ReadFromJsonAsync<GetPersonDto>();
        updated!.LastName.Should().Be("New");
        updated.MailAddress.Should().Be("newemail@test.com");
    }

    [Fact]
    public async Task UpdatePerson_IdMismatch_Returns400()
    {
        // Arrange
        var create = new CreatePersonCommand("Test", "User", "mismatch@test.com");
        var createdResp = await _client.PostAsJsonAsync("/api/people", create);
        var created = await createdResp.Content.ReadFromJsonAsync<GetPersonDto>();

        var updateCmd = new UpdatePersonCommand(created!.Id + 999, "Test", "User", "mismatch@test.com");

        // Act
        var updateResp = await _client.PutAsJsonAsync($"/api/people/{created.Id}", updateCmd);

        // Assert
        updateResp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeletePerson_Works()
    {
        // Arrange
        var create = new CreatePersonCommand("Delete", "Me", "delete@test.com");
        var createdResp = await _client.PostAsJsonAsync("/api/people", create);
        var created = await createdResp.Content.ReadFromJsonAsync<GetPersonDto>();

        // Act
        var delResp = await _client.DeleteAsync($"/api/people/{created!.Id}");

        // Assert
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResp = await _client.GetAsync($"/api/people/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

