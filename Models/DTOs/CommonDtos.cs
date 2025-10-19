namespace SimplyTrack.Api.Models.DTOs
{
    public class ErrorDto
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public ErrorDto? Error { get; set; }

        public static ApiResponse<T> SuccessResult(T data)
        {
            return new ApiResponse<T> { Success = true, Data = data };
        }

        public static ApiResponse<T> ErrorResult(string code, string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Error = new ErrorDto { Code = code, Message = message }
            };
        }
    }
}