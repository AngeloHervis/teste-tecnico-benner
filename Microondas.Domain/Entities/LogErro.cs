namespace Microondas.Domain.Entities;

public class LogErro
{
    public int Id { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? InnerException { get; set; }
    public DateTime DataOcorrencia { get; set; } = DateTime.Now;
    public string? InformacoesRelevantes { get; set; }
}
