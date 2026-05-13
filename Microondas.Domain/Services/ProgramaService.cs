using Microondas.Domain.Constants;
using Microondas.Domain.Entities;
using Microondas.Domain.Interfaces;
using Microondas.Domain._Base;

namespace Microondas.Domain.Services;

public class ProgramaService(IProgramaRepository repository) : IProgramaService
{
    public async Task<ServiceResult<IEnumerable<ProgramaAquecimento>>> ListarAsync()
    {
        var programas = await repository.ObterTodosAsync();
        return ServiceResult<IEnumerable<ProgramaAquecimento>>.Success(programas);
    }

    public async Task<bool> ExisteCaractereAsync(char caractere)
    {
        return await repository.ExisteCaractereAsync(caractere);
    }

    public async Task<bool> ExisteNomeAsync(string nome)
    {
        return await repository.ExisteNomeAsync(nome);
    }

    public async Task<ServiceResult<ProgramaAquecimento>> CadastrarAsync(ProgramaAquecimento programa)
    {
        if (programa.CaractereAquecimento == ValoresPadrao.CaracterePadrao)
            return ServiceResult<ProgramaAquecimento>.UnprocessableEntity($"O caractere '{ValoresPadrao.CaracterePadrao}' é reservado para o aquecimento padrão.");

        if (await repository.ExisteCaractereAsync(programa.CaractereAquecimento))
            return ServiceResult<ProgramaAquecimento>.UnprocessableEntity($"O caractere '{programa.CaractereAquecimento}' já está sendo usado por outro programa.");

        if (await repository.ExisteNomeAsync(programa.Nome))
            return ServiceResult<ProgramaAquecimento>.UnprocessableEntity($"Já existe um programa cadastrado com o nome '{programa.Nome}'.");

        await repository.AdicionarAsync(programa);
        return ServiceResult<ProgramaAquecimento>.Success(programa);
    }
}
