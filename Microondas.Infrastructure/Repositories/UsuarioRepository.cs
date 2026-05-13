using Microondas.Domain.Entities;
using Microondas.Domain.Interfaces;
using Microondas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Microondas.Infrastructure.Repositories;

public class UsuarioRepository(ApplicationDbContext context) : IUsuarioRepository
{
    public async Task<Usuario?> ObterPorNomeAsync(string nome)
    {
        return await context.Usuarios
            .AsNoTracking()
            .Where(u => u.Nome == nome && u.Ativo)
            .FirstOrDefaultAsync();
    }

    public async Task AtualizarAsync(Usuario usuario)
    {
        context.Usuarios.Update(usuario);
        await context.SaveChangesAsync();
    }
}

