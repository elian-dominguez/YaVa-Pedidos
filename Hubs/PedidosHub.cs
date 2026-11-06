using Microsoft.AspNetCore.SignalR;
using YaVa.Data;

namespace YaVa.Hubs;

public class PedidosHub : Hub
{
    private static readonly string[] Roles =
    {
        SeedDatos.RolCaja,
        SeedDatos.RolCocina,
        SeedDatos.RolMostrador
    };

    public override async Task OnConnectedAsync()
    {
        // El grupo lo decide el servidor a partir del rol del usuario autenticado.
        // El navegador no puede elegirlo: este Hub no expone ningún método para unirse a un grupo.
        // Al desconectarse, SignalR saca a la conexión de sus grupos automáticamente.
        foreach (var rol in Roles)
        {
            if (Context.User?.IsInRole(rol) == true)
                await Groups.AddToGroupAsync(Context.ConnectionId, rol);
        }

        await base.OnConnectedAsync();
    }
}
