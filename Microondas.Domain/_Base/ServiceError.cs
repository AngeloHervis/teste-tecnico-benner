namespace Microondas.Domain._Base;

public class ServiceError(string code, string message)
{
    public string Code { get; set; } = code;
    public string Message { get; set; } = message;
}