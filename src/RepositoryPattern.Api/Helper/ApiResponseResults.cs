namespace RepositoryPattern.Api.Helper;

public static class ApiResponseResults
{
    public static IResult Ok<T>(T? data, string? message = null)
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
        return Results.Json(response, statusCode: StatusCodes.Status200OK);
    }

    public static IResult Created<T>(T? data, string? message = null)
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
        return Results.Json(response, statusCode: StatusCodes.Status201Created);
    }

    public static IResult NotFound(string? message = null, string? error = null)
    {
        var response = new ApiResponse<object>
        {
            Success = false,
            Message = message,
            Error = "Not Found"
        };
        return Results.Json(response, statusCode: StatusCodes.Status404NotFound);
    }

    public static IResult Fail(int statusCode, string error, string? message = null)
    {
        var response = new ApiResponse<object>
        {
            Success = false,
            Message = message,
            Error = error
        };
        return Results.Json(response, statusCode: statusCode);
    }
}