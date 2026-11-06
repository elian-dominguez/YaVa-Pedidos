using System.Security.Claims;
using YaVa.Data;

namespace YaVa.Services;

/// <summary>
/// TEMPORAL, solo para desarrollo: permite probar los grupos de SignalR antes de que
/// esté el login real. Borrar este archivo y su llamada en Program.cs al mergear el login.
///
/// Uso: abrir /dev/rol/Cocina (o Caja, o Mostrador) y esa ventana pasa a tener ese rol.
/// </summary>
public static class UsuarioDePrueba
{
    private const string NombreCookie = "dev-rol";

    private static readonly string[] Roles =
    {
        SeedDatos.RolCaja,
        SeedDatos.RolCocina,
        SeedDatos.RolMostrador
    };

    public static void UsarUsuarioDePrueba(this WebApplication app)
    {
        app.MapGet("/dev/rol/{rol}", (string rol, HttpContext ctx) =>
        {
            if (!Roles.Contains(rol))
                return Results.BadRequest("Rol inválido. Usar: " + string.Join(", ", Roles));

            ctx.Response.Cookies.Append(NombreCookie, rol);
            return Results.Redirect("/");
        });

        app.Use(async (ctx, next) =>
        {
            // Si ya hay un usuario real autenticado, no se toca.
            if (ctx.User.Identity?.IsAuthenticated != true
                && ctx.Request.Cookies[NombreCookie] is { } rol
                && Roles.Contains(rol))
            {
                var identidad = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Name, "prueba-" + rol),
                        new Claim(ClaimTypes.Role, rol)
                    },
                    authenticationType: "UsuarioDePrueba");

                ctx.User = new ClaimsPrincipal(identidad);
            }

            await next();
        });
    }
}