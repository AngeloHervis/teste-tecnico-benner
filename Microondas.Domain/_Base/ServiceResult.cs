namespace Microondas.Domain._Base;

public class ServiceResult<TValue>
{
    public bool IsError { get; private init; }
    public int StatusCode { get; private init; }
    public List<ServiceError>? Errors { get; private init; }
    public TValue? Data { get; private init; }
    
    public static ServiceResult<TValue> Success(TValue data)
        => new ()
        {
            IsError = false,
            StatusCode = 200,
            Data = data
        };
    
    public static ServiceResult<TValue> Forbidden(string message)
        => new ()
        {
            IsError = true,
            StatusCode = 403,
            Errors = [new ServiceError("NAO_AUTORIZADO", message)]
        };

    public static ServiceResult<TValue> BadRequest(string message)
        => new ()
        {
            IsError = true,
            StatusCode = 400,
            Errors = [new ServiceError("REQUISICAO_INVALIDA", message)]
        };

    public static ServiceResult<TValue> NotFound(string message)
        => new ()
        {
            IsError = true,
            StatusCode = 404,
            Errors = [new ServiceError("NAO_ENCONTRADO", message)]
        };

    public static ServiceResult<TValue> UnprocessableEntity(string message)
        => new ()
        {
            IsError = true,
            StatusCode = 422,
            Errors = [new ServiceError("ERRO_VALIDACAO", message)]
        };
}