using Microondas.Domain.Entities;
using Microondas.Domain.Interfaces;

namespace Microondas.Infrastructure.State;

public class MicroondasStateProvider : IMicroondasStateProvider
{
    private readonly Dictionary<string, MaquinaMicroondas> _userStates = new();
    private readonly Lock _lockObj = new();

    public MaquinaMicroondas ObterMaquinaDoUsuario(string usuarioId)
    {
        lock (_lockObj)
        {
            if (_userStates.TryGetValue(usuarioId, out var maquina))
                return maquina;
            
            maquina = new MaquinaMicroondas();
            _userStates[usuarioId] = maquina;
            return maquina;
        }
    }

    public IEnumerable<MaquinaMicroondas> ObterTodasAsMaquinas()
    {
        lock (_lockObj)
            return _userStates.Values.ToList();
    }
}
