namespace Microondas.Domain.Dtos;

public record MicroondasStatusDto(
    int TempoRestanteSegundos,
    string TempoFormatado,
    string VisorAquecimento,
    string Estado
);
