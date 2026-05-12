using Microondas.Domain.Dtos;
using Microondas.Domain.Entities;

namespace Microondas.Domain.Mappers;

public static class ProgramaMapper
{
    public static ProgramaAquecimento ParaEntidade(this ProgramaDto dto)
    {
        return new ProgramaAquecimento(
            dto.Nome,
            dto.Alimento,
            dto.TempoSegundos,
            dto.Potencia,
            dto.CaractereAquecimento,
            dto.Instrucoes,
            dto.EhPadrao
        );
    }
}
