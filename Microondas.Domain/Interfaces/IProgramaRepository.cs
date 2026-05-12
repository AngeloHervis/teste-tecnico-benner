using Microondas.Domain.Entities;

namespace Microondas.Domain.Interfaces;

public interface IProgramaRepository
{
    IEnumerable<ProgramaAquecimento> ObterTodos();
}
