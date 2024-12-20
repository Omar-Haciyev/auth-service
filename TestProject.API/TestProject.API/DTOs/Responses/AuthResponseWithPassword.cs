namespace TestProject.API.DTOs.Responses;

public record AuthResponseWithPassword(string Token, string PasswordHash,string UserId);