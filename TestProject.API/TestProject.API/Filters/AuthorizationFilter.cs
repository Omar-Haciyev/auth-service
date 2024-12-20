using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using TestProject.API.DTOs.Responses;
using TestProject.API.Enums;
using TestProject.API.Repositories.Interfaces;
using WebExtensions.Helpers;

namespace TestProject.API.Filters;

public class AuthorizationFilter(Roles role) : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var repository = context.HttpContext.RequestServices.GetService<ISecurityRepository>();

        if (repository == null)
        {
            context.Result = Respondent.Error(ErrorMsgEnum.SomethingWentWrong);
            return;
        }

        var token = context.HttpContext.Request.Headers["token"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(token))
        {
            context.Result = Respondent.Error(ErrorMsgEnum.TokenIsNotDefined);
            return;
        }

        var result = await repository.VerifyAndFetchTokenDetailsAsync(token);

        if (string.IsNullOrEmpty(result))
        {
            context.Result = Respondent.Error(ErrorMsgEnum.TokenIsNotValid);
            return;
        }

        var resultModel = JsonSerializer.Deserialize<TokenDetailsResponse>(result);

        if (resultModel == null || string.IsNullOrEmpty(resultModel.Token))
        {
            context.Result = Respondent.Error(ErrorMsgEnum.TokenIsNotValid);
            return;
        }

        /*
        if (resultModel.ExpiredDate == null || resultModel.ExpiredDate <= DateTime.UtcNow)
        {
            context.Result = Respondent.Error(ErrorMsgEnum.TokenIsExpired);
            return;
        }
        */

        if (role == Roles.AuthorizeUser)
        {
            if (resultModel.Role == Roles.Client.ToString() || resultModel.Role == Roles.Admin.ToString())
            {
                return;
            }
        }

        if (resultModel.Role == role.ToString())
        {
            return;
        }

        context.Result = Respondent.Error(ErrorMsgEnum.AccessDenied);
    }
}