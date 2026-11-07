using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YaVa.Data;
using YaVa.Models;
using YaVa.Services;

namespace YaVa.Controllers;

public class PedidosController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly NotificadorPedidos _notificador;

    public PedidosController(ApplicationDbContext db, NotificadorPedidos notificador)
    {
        _db = db;
        _notificador = notificador;
    }
    [Authorize(Roles = "Administrador,Caja,Cocina,Mostrador")]
    public async Task<IActionResult> Index()
    {
        return View(await PedidosEnCursoAsync());
    }
    [Authorize(Roles = "Administrador,Caja,Cocina,Mostrador")]
    public async Task<IActionResult> Tablero()
    {
        return PartialView("_Tablero", await PedidosEnCursoAsync());
    }

    [Authorize(Roles = "Administrador,Caja")]
    public async Task<IActionResult> Create()
    {
        if (!User.IsInRole("Administrador") &&
            !User.IsInRole("Caja"))
        {
            return Forbid();
        }

        return View(await ArmarFormularioAsync());
    }

    [Authorize(Roles = "Administrador,Caja")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NuevoPedidoViewModel vm)
    {
        if (!User.IsInRole("Administrador") &&
            !User.IsInRole("Caja"))
        {
            return Forbid();
        }

        var elegidos = vm.Items.Where(i => i.Cantidad > 0).ToList();
        if (elegidos.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Elegí al menos un producto.");
        }

        if (!ModelState.IsValid)
        {
            return View(await ArmarFormularioAsync(vm));
        }

        var ids = elegidos.Select(i => i.ProductoId).ToList();
        var productos = await _db.Productos
            .Where(p => ids.Contains(p.Id) && p.Activo)
            .ToDictionaryAsync(p => p.Id);

        var pedido = new Pedido
        {
            Mesa = NormalizarMesa(vm.Mesa),
            CreadoPor = User.Identity?.Name ?? "",
            CreadoEn = DateTime.Now,
            Estado = EstadoPedido.Pendiente
        };

        foreach (var item in elegidos)
        {
            if (!productos.TryGetValue(item.ProductoId, out var producto))
            {
                continue;
            }

            pedido.Items.Add(new PedidoItem
            {
                ProductoId = producto.Id,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio
            });
        }

        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync();

        await _notificador.NotificarAsync(pedido);

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador,Cocina,Mostrador")]
    public async Task<IActionResult> CambiarEstado(int id, EstadoPedido estado)
    {
        var rolNecesario = RolQuePuedePasarA(estado);

        if (rolNecesario is null)
        {
            return BadRequest();
        }

        if (!User.IsInRole("Administrador") && !User.IsInRole(rolNecesario))
        {
            return Forbid();
        }
        
        var pedido = await _db.Pedidos.FindAsync(id);
        
        if (pedido is null)
        {
            return NotFound();
        }

        if (estado != SiguienteEstado(pedido.Estado))
        {
            return BadRequest();
        }

        pedido.Estado = estado;

        if (estado == EstadoPedido.Entregado)
        {
            pedido.EntregadoEn = DateTime.Now;
        }

        await _db.SaveChangesAsync();

        await _notificador.NotificarAsync(pedido);

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador,Caja,Cocina,Mostrador")]
    public async Task<IActionResult> Historial()
    {
        var pedidos = await _db.Pedidos
            .Include(p => p.Items)
            .ThenInclude(i => i.Producto)
            .Where(p => p.Estado == EstadoPedido.Entregado)
            .OrderByDescending(p => p.EntregadoEn ?? p.CreadoEn)
            .ToListAsync();

        return View(pedidos);
    }

    private async Task<List<Pedido>> PedidosEnCursoAsync()
    {
        return await _db.Pedidos
            .Include(p => p.Items)
            .ThenInclude(i => i.Producto)
            .Where(p => p.Estado != EstadoPedido.Entregado)
            .OrderBy(p => p.CreadoEn)
            .ToListAsync();
    }

    private async Task<NuevoPedidoViewModel> ArmarFormularioAsync(NuevoPedidoViewModel? vm = null)
    {
        var productos = await _db.Productos.Where(p => p.Activo).OrderBy(p => p.Nombre).ToListAsync();
        var cantidades = vm?.Items.ToDictionary(i => i.ProductoId, i => i.Cantidad)
                         ?? new Dictionary<int, int>();

        return new NuevoPedidoViewModel
        {
            Mesa = vm?.Mesa ?? "",
            Items = productos.Select(p => new ItemViewModel
            {
                ProductoId = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                Cantidad = cantidades.TryGetValue(p.Id, out var c) ? c : 0
            }).ToList()
        };
    }

    private static string NormalizarMesa(string mesa)
    {
        mesa = mesa.Trim();

        if (mesa.StartsWith("Mesa ", StringComparison.OrdinalIgnoreCase))
        {
            mesa = mesa["Mesa ".Length..].Trim();
        }

        return mesa;
    }

    public static EstadoPedido? SiguienteEstado(EstadoPedido actual) => actual switch
    {
        EstadoPedido.Pendiente => EstadoPedido.EnPreparacion,
        EstadoPedido.EnPreparacion => EstadoPedido.Listo,
        EstadoPedido.Listo => EstadoPedido.Entregado,
        _ => null
    };

    public static string? RolQuePuedePasarA(EstadoPedido estado) => estado switch
    {
        EstadoPedido.EnPreparacion => SeedDatos.RolCocina,
        EstadoPedido.Listo => SeedDatos.RolCocina,
        EstadoPedido.Entregado => SeedDatos.RolMostrador,
        _ => null
    };
}
