using Microondas.Domain.Entities;

namespace Microondas.Domain.Interfaces;

public interface IMicroondasStateProvider
{
    MaquinaMicroondas ObterMaquinaDoUsuario(string usuarioId);
    IEnumerable<MaquinaMicroondas> ObterTodasAsMaquinas();
}