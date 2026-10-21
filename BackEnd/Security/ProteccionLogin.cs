using Microsoft.Extensions.Caching.Memory;

namespace ControlVehicular.Security;

public interface IProteccionLogin
{
    bool EstaBloqueado(string clave);
    void RegistrarFallo(string clave);
    void RegistrarExito(string clave);
}

public sealed class ProteccionLogin : IProteccionLogin
{
    private const int MaximosFallos = 5;
    private static readonly TimeSpan Ventana = TimeSpan.FromMinutes(15);
    private readonly IMemoryCache _cache;
    private readonly object _candado = new();

    public ProteccionLogin(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool EstaBloqueado(string clave)
    {
        lock (_candado)
        {
            return _cache.TryGetValue(Clave(clave), out EstadoLogin? estado)
                   && estado is not null
                   && estado.Fallos >= MaximosFallos;
        }
    }

    public void RegistrarFallo(string clave)
    {
        var id = Clave(clave);
        lock (_candado)
        {
            if (!_cache.TryGetValue(id, out EstadoLogin? estado) || estado is null)
            {
                estado = new EstadoLogin(1, DateTimeOffset.UtcNow.Add(Ventana));
                _cache.Set(id, estado, new MemoryCacheEntryOptions { AbsoluteExpiration = estado.Expira });
                return;
            }

            var actualizado = estado with { Fallos = estado.Fallos + 1 };
            _cache.Set(id, actualizado, new MemoryCacheEntryOptions { AbsoluteExpiration = estado.Expira });
        }
    }

    public void RegistrarExito(string clave)
    {
        lock (_candado)
        {
            _cache.Remove(Clave(clave));
        }
    }

    private static string Clave(string valor) => "login:" + valor.Trim().ToLowerInvariant();

    private sealed record EstadoLogin(int Fallos, DateTimeOffset Expira);
}
