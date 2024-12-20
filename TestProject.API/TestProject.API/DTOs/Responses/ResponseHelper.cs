using Microsoft.AspNetCore.Mvc;

namespace TestProject.API.DTOs.Responses;

public static class ResponseHelper
{
    public static CustomResponse<T?> Error<T>(int code, string errorMessage)
    {
        return new CustomResponse<T?>
        {
            Result = new CustomResponse<T?>.ResultModel
            {
                Status = false,
                Code = code,
                Time = 0,
                Error = true,
                ErrorMsg = errorMessage
            },
            Data = default
        };
    }

    public static CustomResponse<T?> Success<T>(T? data)
    {
        return new CustomResponse<T?>
        {
            Result = new CustomResponse<T?>.ResultModel
            {
                Status = true,
                Code = 200,
                Time = 0,
                Error = false,
                ErrorMsg = null
            },
            Data = data
        };
    }
}