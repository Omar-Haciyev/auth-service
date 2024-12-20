using System.Data;
using TestProject.API.DTOs.Requests;
using TestProject.API.Repositories.Interfaces;
using WebExtensions.Helpers;

namespace TestProject.API.Repositories.Classes;

public class SecurityRepository(string connectionString) : RepositoryHelper(connectionString), ISecurityRepository
{
    public async Task<string?> GenerateTokenAsync(string platformKey)
    {
        Command.Name = "sp_generate_token";
        Command.AddParameter("platform_key", platformKey);

        return await base.ExecuteCommandAsync<string>();
    }

    public async Task<string?> VerifyAndFetchTokenDetailsAsync(string token)
    {
        Command.Name = "sp_validate_token_and_return_info";
        Command.AddParameter("token", token);

        return await base.ExecuteCommandAsync<string>();
    }

    public async Task<string?> SignUpUserAsync(string token, SignUpRequest request)
    {
        Command.Name = "sp_client_sign_up";
        Command.AddParameter("token", token);
        Command.AddParameter("name", request.Name);
        Command.AddParameter("dob", request.DateOfBirth);
        Command.AddParameter("password_hash", request.PasswordHash);

        return await base.ExecuteCommandAsync<string>();
    }

    public async Task<string?> GetSignInDetailsAsync(string token, string name)
    {
        Command.Name = "sp_client_sign_in";
        Command.AddParameter("token", token);
        Command.AddParameter("name", name);

        return await base.ExecuteCommandAsync<string>();
    }
    
    public async Task<bool> UpdateUserSessionAsync(string token, string userId)
    {
        Command.Name = "sp_client_sign_in_update";
        Command.AddParameter("token", token);
        Command.AddParameter("user_id", userId);

        Command.ReturnValue = new ReturnValueOption("sql_res", SqlDbType.Bit);

        return await base.ExecuteCommandAsync<bool>();
    }
    
    public async Task<string?> GetUserDataAsync(string token)
    {
        Command.Name = "sp_get_user_date";
        Command.AddParameter("token", token);

        return await base.ExecuteCommandAsync<string>();
    }
}