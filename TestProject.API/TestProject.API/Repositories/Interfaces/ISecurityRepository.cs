using TestProject.API.DTOs.Requests;

namespace TestProject.API.Repositories.Interfaces;

public interface ISecurityRepository
{
    Task<string?> GenerateTokenAsync(string platformKey);
    Task<string?> VerifyAndFetchTokenDetailsAsync(string token);
    Task<string?> SignUpUserAsync(string token, SignUpRequest request);
    Task<string?> GetSignInDetailsAsync(string token, string name);
    Task<bool> UpdateUserSessionAsync(string token, string userId);
    Task<string?> GetUserDataAsync(string token);
}