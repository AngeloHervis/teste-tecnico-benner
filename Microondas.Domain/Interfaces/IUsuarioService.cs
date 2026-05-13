using Microondas.Domain._Base;

namespace Microondas.Domain.Interfaces;

public interface IUsuarioService
{
    Task<ServiceResult<string>> RealizarLoginAsync(string username, string password);
    Task<ServiceResult<string>> AtualizarSenhaAsync(string username, string novaSenha);
}

