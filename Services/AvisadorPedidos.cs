using Microsoft.AspNetCore.SignalR;
using YaVa.Data;
using YaVa.Hubs;
using YaVa.Models;

namespace YaVa.Services;

/// <summary>
/// Avisa que un pedido cambió, usando los grupos de SignalR (uno por rol).
/// Manda dos mensajes distintos:
///  - "TableroActualizado": a todos los puestos, para que refresquen su tablero.
///  - "PedidoActualizado": solo al puesto que tiene que actuar, para el aviso destacado.
/// </summary>
public class AvisadorPedidos
{
    private static readonly string[] TodosLosRoles =
    {
        SeedDatos.RolCaja,
        SeedDatos.RolCocina,
        SeedDatos.RolMostrador
    };
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

        await _hub.Clients.Groups(TodosLosRoles).SendAsync("TableroActualizado");

        await _hub.Clients.Groups(GruposQueAvisar(pedido.Estado)).SendAsync("PedidoActualizado", aviso);
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
