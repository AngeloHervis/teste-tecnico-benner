using Microondas.Domain.Entities;

namespace Microondas.Domain.Interfaces;

public interface IProgramaService
{
    Task<IEnumerable<ProgramaAquecimento>> ListarAsync();
    Task CadastrarAsync(ProgramaAquecimento programa);
}
