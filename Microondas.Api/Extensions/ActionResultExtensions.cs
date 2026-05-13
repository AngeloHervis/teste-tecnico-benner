using Microondas.Api.Dtos;
using Microondas.Domain._Base;
using Microsoft.AspNetCore.Mvc;

namespace Microondas.Api.Extensions;

public static class ActionResultExtensions
{
    public static async Task<IActionResult> ToActionResultAsync<T>(this Task<ServiceResult<T>> result)
    {
        var resultado = await result;

        if (!resultado.IsError)
            return new ObjectResult(resultado.Data)
            {
                StatusCode = resultado.StatusCode
            };

        var message = string.Join("; ", resultado.Errors?.Select(e => e.Message) ?? []);
        var code = resultado.Errors?.FirstOrDefault()?.Code;

        return new ObjectResult(new ErroDto(message, code))
        {
            StatusCode = resultado.StatusCode
        };
    }
}