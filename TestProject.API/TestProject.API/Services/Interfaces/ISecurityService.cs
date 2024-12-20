using TestProject.API.DTOs.Requests;
using TestProject.API.DTOs.Responses;

namespace TestProject.API.Services.Interfaces;

public interface ISecurityService
{
    Task<CustomResponse<AuthResponse?>> GenerateTokenAsync(string platformKey);
    Task<CustomResponse<AuthResponse?>> SignUpUserAsync(string token, SignUpRequest request);
    Task<CustomResponse<AuthResponse?>> SignInUserAsync(string token, SignInRequest request);
    Task<CustomResponse<UserDataResponse?>> GetUserDataAsync(string token);
}