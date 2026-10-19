using Microsoft.AspNetCore.SignalR;

namespace YaVa.Hubs;

public class PedidosHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Cuando esté el login, acá hay que meter a cada conexión en el grupo de su rol
        // (Caja, Cocina o Mostrador). El grupo lo tiene que decidir el servidor, no el navegador.
        await base.OnConnectedAsync();
    }
}
