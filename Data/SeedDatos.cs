using Microsoft.EntityFrameworkCore;
using YaVa.Models;

namespace YaVa.Data;

public static class SeedDatos
{
    public const string RolCaja = "Caja";
    public const string RolCocina = "Cocina";
    public const string RolMostrador = "Mostrador";

    public static readonly string[] RolesOperativos = { RolCaja, RolCocina, RolMostrador };

    public static async Task InicializarAsync(IServiceProvider servicios)
    {
        var db = servicios.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        if (!await db.Productos.AnyAsync())
        {
            db.Productos.AddRange(
                new Producto { Nombre = "Café", Precio = 1800 },
                new Producto { Nombre = "Cortado", Precio = 2000 },
                new Producto { Nombre = "Medialuna", Precio = 900 },
                new Producto { Nombre = "Tostado de jamón y queso", Precio = 4500 },
                new Producto { Nombre = "Hamburguesa con papas", Precio = 9800 },
                new Producto { Nombre = "Agua saborizada", Precio = 2200 },
                new Producto { Nombre = "Cerveza tirada", Precio = 4200 });

            await db.SaveChangesAsync();
        }
    }
}
