namespace AES.Services;
public class OperationResult
{
    public bool IsSuccess { get; }
    public string Message { get; }

    private OperationResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static OperationResult Success(string message = "Операція успішна") =>
        new(true, message);

    public static OperationResult Fail(string message) =>
        new(false, message);
}