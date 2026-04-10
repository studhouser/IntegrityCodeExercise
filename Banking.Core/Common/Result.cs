namespace Banking.Core.Common;

public class OperationResult<T>
{
    public bool Succeeded { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static OperationResult<T> Success(T data) => new() { Succeeded = true, Data = data };
    public static OperationResult<T> Failure(string message) => new() { Succeeded = false, ErrorMessage = message };
}