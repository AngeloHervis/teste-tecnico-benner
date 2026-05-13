using Microondas.Domain.Entities;
using Microondas.Domain._Base;

namespace Microondas.Domain.Interfaces;

public interface IProgramaService
{
    Task<ServiceResult<IEnumerable<ProgramaAquecimento>>> ListarAsync();
    Task<bool> ExisteCaractereAsync(char caractere);
    Task<bool> ExisteNomeAsync(string nome);
    Task<ServiceResult<ProgramaAquecimento>> CadastrarAsync(ProgramaAquecimento programa);
}
