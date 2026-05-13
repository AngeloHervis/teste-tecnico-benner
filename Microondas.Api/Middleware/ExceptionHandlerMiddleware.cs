using Microondas.Api.Dtos;
using Microondas.Domain.Exceptions;
using Microondas.Domain.Entities;
using Microondas.Infrastructure.Data;

namespace Microondas.Api.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        try
        {
            await next(context);
        }
        catch (ValidacaoMicroondasException ex)
        {
            logger.LogWarning(ex, "Regra de Negócio: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex, 400);
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu uma falha sistêmica crítica. Path: {Path} | Method: {Method} | Message: {Message}", 
                context.Request.Path, context.Request.Method, ex.Message);

            await SalvarLogBancoAsync(context, dbContext, ex);
            await HandleExceptionAsync(context, ex, 500);
        }
    }

    private static async Task SalvarLogBancoAsync(HttpContext context, ApplicationDbContext dbContext, Exception ex)
    {
        try
        {
            var log = new LogErro
            {
                Mensagem = ex.Message,
                StackTrace = ex.StackTrace,
                InnerException = ex.InnerException?.Message,
                DataOcorrencia = DateTime.Now,
                InformacoesRelevantes = $"Path: {context.Request.Path} | Method: {context.Request.Method}"
            };

            dbContext.LogsErro.Add(log);
            await dbContext.SaveChangesAsync();
        }
        catch
        {
            // ignored
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex, int statusCode)
    {
        if (ex is OperationCanceledException || ex is TaskCanceledException || context.Response.HasStarted)
            return Task.CompletedTask;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var code = statusCode == 400 ? "VALIDATION_ERROR" : "INTERNAL_SERVER_ERROR";
        var response = new ErroDto(ex.Message, code);

        return context.Response.WriteAsJsonAsync(response);
    }
}