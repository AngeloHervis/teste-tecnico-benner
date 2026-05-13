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

    public async Task<bool> ExisteCaractereAsync(char caractere)
    {
        return await repository.ExisteCaractereAsync(caractere);
    }

    public async Task<bool> ExisteNomeAsync(string nome)
    {
        return await repository.ExisteNomeAsync(nome);
    }

    public async Task CadastrarAsync(ProgramaAquecimento programa)
    {
        if (await repository.ExisteCaractereAsync(programa.CaractereAquecimento))
            throw new ValidacaoMicroondasException($"O caractere '{programa.CaractereAquecimento}' já está sendo usado por outro programa.");

        if (await repository.ExisteNomeAsync(programa.Nome))
            throw new ValidacaoMicroondasException($"Já existe um programa cadastrado com o nome '{programa.Nome}'.");

        await repository.AdicionarAsync(programa);
    }
}
