using Microondas.Domain.Interfaces;
using Microondas.Domain._Base;

namespace Microondas.Domain.Services;

public class UsuarioService(
    IUsuarioRepository usuarioRepository, 
    ICryptoProvider cryptoProvider, 
    ITokenService tokenService) : IUsuarioService
{
    public async Task<ServiceResult<string>> RealizarLoginAsync(string username, string password)
    {
        var usuario = await usuarioRepository.ObterPorNomeAsync(username);
        
        if (usuario == null || cryptoProvider.ComputeHash(password) != usuario.SenhaHash)
            return ServiceResult<string>.Forbidden("Usuário ou senha inválidos.");
        
        var token = tokenService.GenerateToken(username);
        return ServiceResult<string>.Success(token);
    }

    public async Task<ServiceResult<string>> AtualizarSenhaAsync(string username, string novaSenha)
    {
        var usuario = await usuarioRepository.ObterPorNomeAsync(username);
        if (usuario == null)
            return ServiceResult<string>.NotFound("Usuário não encontrado.");

        var novoUsuario = new Microondas.Domain.Entities.Usuario
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            SenhaHash = cryptoProvider.ComputeHash(novaSenha),
            DataCriacao = usuario.DataCriacao,
            Ativo = usuario.Ativo
        };

        await usuarioRepository.AtualizarAsync(novoUsuario);
        return ServiceResult<string>.Success("Credenciais atualizadas com sucesso.");
    }
}

