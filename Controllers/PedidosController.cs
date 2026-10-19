using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YaVa.Data;
using YaVa.Models;
using YaVa.Services;

namespace YaVa.Controllers;

public class PedidosController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly AvisadorPedidos _avisador;

    public PedidosController(ApplicationDbContext db, AvisadorPedidos avisador)
    {
        _db = db;
        _avisador = avisador;
    }

    public async Task<IActionResult> Index()
    {
        return View(await PedidosEnCursoAsync());
    }

    public async Task<IActionResult> Tablero()
    {
        return PartialView("_Tablero", await PedidosEnCursoAsync());
    }

    public async Task<IActionResult> Create()
    {
        return View(await ArmarFormularioAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NuevoPedidoViewModel vm)
    {
        var elegidas = vm.Lineas.Where(l => l.Cantidad > 0).ToList();
        if (elegidas.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Elegí al menos un producto.");
        }

        if (!ModelState.IsValid)
        {
            return View(await ArmarFormularioAsync(vm));
        }

        var ids = elegidas.Select(l => l.ProductoId).ToList();
        var productos = await _db.Productos
            .Where(p => ids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var pedido = new Pedido
        {
            Mesa = vm.Mesa,
            CreadoPor = "",
            CreadoEn = DateTime.Now,
            Estado = EstadoPedido.Pendiente
        };

        foreach (var linea in elegidas)
        {
            if (!productos.TryGetValue(linea.ProductoId, out var producto))
            {
                continue;
            }

            pedido.Items.Add(new PedidoItem
            {
                ProductoId = producto.Id,
                Cantidad = linea.Cantidad,
                PrecioUnitario = producto.Precio
            });
        }

        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync();

        await _avisador.AvisarAsync(pedido);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, EstadoPedido estado)
    {
        if (RolQuePuedePasarA(estado) is null)
        {
            return BadRequest();
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
        await _db.SaveChangesAsync();

        await _avisador.AvisarAsync(pedido);

        return RedirectToAction(nameof(Index));
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
        var productos = await _db.Productos.OrderBy(p => p.Nombre).ToListAsync();
        var cantidades = vm?.Lineas.ToDictionary(l => l.ProductoId, l => l.Cantidad)
                         ?? new Dictionary<int, int>();

        return new NuevoPedidoViewModel
        {
            Mesa = vm?.Mesa ?? "",
            Lineas = productos.Select(p => new LineaViewModel
            {
                ProductoId = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                Cantidad = cantidades.TryGetValue(p.Id, out var c) ? c : 0
            }).ToList()
        };
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
