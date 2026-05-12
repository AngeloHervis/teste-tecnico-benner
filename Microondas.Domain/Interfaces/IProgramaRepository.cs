using Microondas.Domain.Entities;

namespace Microondas.Domain.Interfaces;

public interface IProgramaRepository
{
    Task<IEnumerable<ProgramaAquecimento>> ObterTodosAsync();
    Task AdicionarAsync(ProgramaAquecimento programa);
    Task<bool> ExisteCaractereAsync(char caractere);
}
