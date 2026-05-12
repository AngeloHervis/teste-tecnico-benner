using Microondas.Domain.Entities;
using Microondas.Domain.Entities.Programas;
using Microondas.Domain.Interfaces;

namespace Microondas.Domain.Services;

public class ProgramaRepository : IProgramaRepository
{
    private readonly List<ProgramaAquecimento> _programasPreDefinidos =
    [
        new ProgramaPipoca(),
        new ProgramaLeite(),
        new ProgramaCarneDeBoi(),
        new ProgramaFrango(),
        new ProgramaFeijao()
    ];

    public IEnumerable<ProgramaAquecimento> ObterTodos()
    {
        return _programasPreDefinidos.ToList();
    }
}
