using Microsoft.AspNetCore.Mvc;
using TestProject.API.DTOs.Requests;
using TestProject.API.DTOs.Responses;
using TestProject.API.Enums;
using TestProject.API.Filters;
using TestProject.API.Services.Interfaces;
using WebExtensions.Helpers;

namespace TestProject.API.Controllers;

[Route("api/security")]
public class SecurityController(ISecurityService securityService) : ControllerBase
{
    /// <summary>
    /// Generates a session token for a platform.
    /// </summary>
    /// <param name="platformKey">Key of the platform requesting the token.</param>
    /// <returns>A response containing the generated token.</returns>
    /// <response code="200">Token generated successfully.</response>
    /// <response code="400">Invalid platform key.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route("tokens")]
    [ProducesResponseType(typeof(ResponseModel<AuthResponse?>), 200)]
    [Produces("application/json")]
    public async Task<IActionResult> GenerateTokenAsync([FromHeader] string platformKey)
    {
        var response = await securityService.GenerateTokenAsync(platformKey);

        return response.Result.Error ? StatusCode(response.Result.Code, response) : Ok(response);
    }

    /// <summary>
    /// Registers a new user with the provided details.
    /// </summary>
    /// <param name="token">Session token.</param>
    /// <param name="request">Sign-up details of the user.</param>
    /// <returns>A response containing the generated token or error details.</returns>
    [HttpPost]
    [Route("registerUser")]
    [AuthorizationFilter(Roles.Guest)]
    [ProducesResponseType(typeof(ResponseModel<AuthResponse?>), 200)]
    [Produces("application/json")]
    public async Task<IActionResult> SignUpUserAsync([FromHeader] string token, [FromBody] SignUpRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errorResponse = ResponseHelper.Error<AuthResponse?>(400, "Parameters not valid.");
            return BadRequest(errorResponse);
        }

        var response = await securityService.SignUpUserAsync(token, request);
        return !response.Result.Error ? StatusCode(response.Result.Code, response) : Ok(response);
    }

    /// <summary>
    /// Authenticates a user.
    /// </summary>
    /// <param name="token">Session token.</param>
    /// <param name="request">Login details of the user.</param>
    /// <returns>A response containing the generated token or error details.</returns>
    [HttpPost]
    [Route("login")]
    [AuthorizationFilter(Roles.Guest)]
    [ProducesResponseType(typeof(ResponseModel<AuthResponse?>), 200)]
    [Produces("application/json")]
    public async Task<IActionResult> SignInUserAsync([FromHeader] string token, [FromBody] SignInRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errorResponse = ResponseHelper.Error<AuthResponse?>(400, "Parameters not valid.");
            return BadRequest(errorResponse);
        }

        var response = await securityService.SignInUserAsync(token, request);
        return !response.Result.Error ? StatusCode(response.Result.Code, response) : Ok(response);
    }

    /// <summary>
    /// Retrieves authenticated user's data.
    /// </summary>
    /// <param name="token">Session token of the user.</param>
    /// <returns>A response containing user data or error details.</returns>
    [HttpGet]
    [Route("me")]
    [AuthorizationFilter(Roles.AuthorizeUser)]
    [ProducesResponseType(typeof(ResponseModel<UserDataResponse?>), 200)]
    [Produces("application/json")]
    public async Task<IActionResult> GetUserDataAsync([FromHeader] string token)
    {
        var response = await securityService.GetUserDataAsync(token);
        return !response.Result.Error ? StatusCode(response.Result.Code, response) : Ok(response);
    }
}
