using Microondas.Domain.Entities;

namespace Microondas.Domain.Interfaces;

public interface IProgramaService
{
    Task<IEnumerable<ProgramaAquecimento>> ListarAsync();
    Task<bool> ExisteCaractereAsync(char caractere);
    Task<bool> ExisteNomeAsync(string nome);
    Task CadastrarAsync(ProgramaAquecimento programa);
}
