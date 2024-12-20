using System.Text.Json.Serialization;

namespace TestProject.API.DTOs.Responses;

public record TokenDetailsResponse(
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("user_id")]
    string UserId,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("expired_date")]
    DateTime ExpiredDate,
    [property: JsonPropertyName("status")] string Status);