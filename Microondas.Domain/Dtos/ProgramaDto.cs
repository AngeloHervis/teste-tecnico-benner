namespace Microondas.Domain.Dtos;

public record ProgramaDto(
    string Nome,
    string Alimento,
    int TempoSegundos,
    int Potencia,
    char CaractereAquecimento,
    string Instrucoes,
    bool EhPadrao
);
