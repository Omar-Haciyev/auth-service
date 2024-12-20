using System.Text.Json;
using TestProject.API.DTOs.Requests;
using TestProject.API.DTOs.Responses;
using TestProject.API.Helpers.Interfaces;
using TestProject.API.Repositories.Interfaces;
using TestProject.API.Services.Interfaces;

namespace TestProject.API.Services.Classes;

public class SecurityService(ISecurityRepository repository, IPasswordHasher passwordHasher) : ISecurityService
{
    public async Task<CustomResponse<AuthResponse?>> GenerateTokenAsync(string platformKey)
    {
        if (string.IsNullOrWhiteSpace(platformKey))
            return ResponseHelper.Error<AuthResponse?>(400, "Platform key is required.");

        if (platformKey.Length != 12)
            return ResponseHelper.Error<AuthResponse?>(400, "Platform key must be exactly 12 characters long.");

        var responseJson = await repository.GenerateTokenAsync(platformKey);

        if (string.IsNullOrEmpty(responseJson))
            return ResponseHelper.Error<AuthResponse?>(404, "Invalid or inactive platform key.");

        var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseJson);

        if (authResponse == null)
            return ResponseHelper.Error<AuthResponse?>(500, "Error processing token response.");

        return ResponseHelper.Success(authResponse);
    }

    public async Task<CustomResponse<AuthResponse?>> SignUpUserAsync(string token, SignUpRequest request)
    {
        var hashedPassword = passwordHasher.HashPassword(request.PasswordHash);

        var requestWithHashedPassword = request with { PasswordHash = hashedPassword };
        
        var responseJson = await repository.SignUpUserAsync(token, requestWithHashedPassword);

        if (string.IsNullOrEmpty(responseJson))
            return ResponseHelper.Error<AuthResponse?>(500, "Error processing sign-up response.");

        var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseJson);

        if (authResponse == null)
            return ResponseHelper.Error<AuthResponse?>(500, "Error parsing sign-up response.");

        return ResponseHelper.Success(authResponse);
    }

    public async Task<CustomResponse<AuthResponse?>> SignInUserAsync(string token, SignInRequest request)
    {
        var responseJson = await repository.GetSignInDetailsAsync(token, request.Name);

        if (string.IsNullOrEmpty(responseJson))
            return ResponseHelper.Error<AuthResponse?>(404, "Invalid credentials.");

        var signInDetails = JsonSerializer.Deserialize<AuthResponseWithPassword>(responseJson);
        if (signInDetails == null)
            return ResponseHelper.Error<AuthResponse?>(500, "Failed to parse the response.");

        if (!passwordHasher.VerifyPassword(signInDetails.PasswordHash, request.Password))
            return ResponseHelper.Error<AuthResponse?>(401, "Invalid credentials.");

        var updateResult = await repository.UpdateUserSessionAsync(token, signInDetails.UserId);
        if (!updateResult)
            return ResponseHelper.Error<AuthResponse?>(500, "Failed to update session.");

        var authResponse = new AuthResponse(signInDetails.Token);
        return ResponseHelper.Success(authResponse);
    }
    
    public async Task<CustomResponse<UserDataResponse?>> GetUserDataAsync(string token)
    {
        var responseJson = await repository.GetUserDataAsync(token);

        var authResponse = JsonSerializer.Deserialize<UserDataResponse>(responseJson!);

        if (authResponse == null)
            return ResponseHelper.Error<UserDataResponse?>(500, "Error processing token response.");

        return ResponseHelper.Success(authResponse);
    }
}