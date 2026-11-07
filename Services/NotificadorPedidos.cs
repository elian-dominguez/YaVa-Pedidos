using Microsoft.AspNetCore.SignalR;
using YaVa.Data;
using YaVa.Hubs;
using YaVa.Models;

namespace YaVa.Services;

/// <summary>
/// Notifica que un pedido cambió, usando los grupos de SignalR (uno por rol).
/// Manda dos mensajes distintos:
///  - "TableroActualizado": a todos los puestos, para que refresquen su tablero.
///  - "PedidoActualizado": solo al puesto que tiene que actuar, para el aviso destacado.
/// </summary>
public class NotificadorPedidos
{
    private static readonly string[] TodosLosRoles =
    {
        SeedDatos.RolCaja,
        SeedDatos.RolCocina,
        SeedDatos.RolMostrador
    };
    private readonly IHubContext<PedidosHub> _hub;

    public NotificadorPedidos(IHubContext<PedidosHub> hub)
    {
        _hub = hub;
    }

    public async Task NotificarAsync(Pedido pedido)
    {
        var aviso = new AvisoPedido
        {
            Id = pedido.Id,
            Mesa = pedido.Mesa,
            Estado = pedido.Estado.Texto()
        };

        await _hub.Clients.Groups(TodosLosRoles).SendAsync("TableroActualizado");

        await _hub.Clients.Groups(GruposQueNotificar(pedido.Estado)).SendAsync("PedidoActualizado", aviso);
    }

    public static string[] GruposQueNotificar(EstadoPedido estado) => estado switch
    {
        EstadoPedido.Pendiente => new[] { SeedDatos.RolCocina },
        EstadoPedido.EnPreparacion => new[] { SeedDatos.RolCaja },
        EstadoPedido.Listo => new[] { SeedDatos.RolCaja, SeedDatos.RolMostrador },
        EstadoPedido.Entregado => new[] { SeedDatos.RolCaja, SeedDatos.RolCocina },
        _ => Array.Empty<string>()
    };
}
