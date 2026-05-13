using Microondas.Domain.Entities;

namespace Microondas.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorNomeAsync(string nome);
    
    Task AtualizarAsync(Usuario usuario);
}

