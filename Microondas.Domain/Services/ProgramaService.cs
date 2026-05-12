using Microondas.Domain.Entities;
using Microondas.Domain.Interfaces;
using Microondas.Domain.Exceptions;

namespace Microondas.Domain.Services;

public class ProgramaService(IProgramaRepository repository) : IProgramaService
{
    public async Task<IEnumerable<ProgramaAquecimento>> ListarAsync()
    {
        return await repository.ObterTodosAsync();
    }

    public async Task CadastrarAsync(ProgramaAquecimento programa)
    {
        if (await repository.ExisteCaractereAsync(programa.CaractereAquecimento))
            throw new ValidacaoMicroondasException($"O caractere '{programa.CaractereAquecimento}' já está sendo usado por outro programa.");

        await repository.AdicionarAsync(programa);
    }
}
