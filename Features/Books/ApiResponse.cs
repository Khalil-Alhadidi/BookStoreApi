namespace BookStoreApi.Features.Books;

public class ApiResponse<T>
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public ApiResponse() { }
    public ApiResponse(bool isSuccessful, string message, T? data = default)
    {
        IsSuccessful = isSuccessful;
        Message = message;
        Data = data;
    }
} 