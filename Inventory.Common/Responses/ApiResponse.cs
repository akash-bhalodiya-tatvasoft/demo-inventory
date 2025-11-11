namespace Inventory.Common.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }
    public object? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(
        T data,
        int statusCode = 200,
        string message = "Success")
        => new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };

    public static ApiResponse<T> Failure(
        int statusCode = 500,
        string message = "Error",
        object errors = null)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors
        };
}
