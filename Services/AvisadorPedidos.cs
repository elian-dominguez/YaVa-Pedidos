using Microsoft.AspNetCore.SignalR;
using YaVa.Data;
using YaVa.Hubs;
using YaVa.Models;

namespace YaVa.Services;

/// <summary>
/// Avisa que un pedido cambió. Hoy le llega a todos los navegadores conectados.
/// Con el login, conviene mandarlo solo al grupo del puesto que lo necesita.
/// </summary>
public class AvisadorPedidos
{
    private readonly IHubContext<PedidosHub> _hub;

    public AvisadorPedidos(IHubContext<PedidosHub> hub)
    {
        _hub = hub;
    }

    public async Task AvisarAsync(Pedido pedido)
    {
        var aviso = new AvisoPedido
        {
            Id = pedido.Id,
            Mesa = pedido.Mesa,
            Estado = pedido.Estado.Texto()
        };

        await _hub.Clients.All.SendAsync("PedidoActualizado", aviso);
    }

    public static string[] GruposQueAvisar(EstadoPedido estado) => estado switch
    {
        EstadoPedido.Pendiente => new[] { SeedDatos.RolCocina },
        EstadoPedido.EnPreparacion => new[] { SeedDatos.RolCaja },
        EstadoPedido.Listo => new[] { SeedDatos.RolCaja, SeedDatos.RolMostrador },
        EstadoPedido.Entregado => new[] { SeedDatos.RolCaja, SeedDatos.RolCocina },
        _ => Array.Empty<string>()
    };
}
