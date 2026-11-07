
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace YaVa.Hubs;

[Authorize(Roles = "Administrador,Caja,Cocina,Mostrador")]
public class PedidosHub : Hub
{
    private readonly UserManager<IdentityUser> _userManager;

    public PedidosHub(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public override async Task OnConnectedAsync()
    {
        var usuario = await _userManager.GetUserAsync(Context.User);

        if (usuario == null ||
            await _userManager.IsLockedOutAsync(usuario))
        {
            Context.Abort();
            return;
        }

        string[] roles =
        {
            "Administrador",
            "Caja",
            "Cocina",
            "Mostrador"
        };

        foreach (var rol in roles)
        {
            if (Context.User?.IsInRole(rol) == true)
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId, rol);
            }
        }

        await base.OnConnectedAsync();
    }
}
