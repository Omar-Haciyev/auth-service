using System.Text.Json.Serialization;

namespace TestProject.API.DTOs.Responses;

public record UserDataResponse(
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("Dob")] DateTime DateOfBirth);