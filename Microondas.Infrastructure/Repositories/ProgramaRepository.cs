using Microondas.Domain.Entities;
using Microondas.Domain.Interfaces;
using Microondas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Microondas.Infrastructure.Repositories;

public class ProgramaRepository(ApplicationDbContext context) : IProgramaRepository
{
    public async Task<IEnumerable<ProgramaAquecimento>> ObterTodosAsync()
    {
        return await context.ProgramasAquecimento.AsNoTracking().ToListAsync();
    }

    public async Task AdicionarAsync(ProgramaAquecimento programa)
    {
        await context.ProgramasAquecimento.AddAsync(programa);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExisteCaractereAsync(char caractere)
    {
        return await context.ProgramasAquecimento.AnyAsync(p => p.CaractereAquecimento == caractere);
    }

    public async Task<bool> ExisteNomeAsync(string nome)
    {
        return await context.ProgramasAquecimento.AnyAsync(p => p.Nome.ToLower() == nome.ToLower());
    }
}
