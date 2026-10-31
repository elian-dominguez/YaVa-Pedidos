using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YaVa.Data;
using YaVa.Models;

namespace YaVa.Controllers
{
    public class ProductosController : Controller
    {

        private readonly ApplicationDbContext _db;

        public ProductosController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _db.Productos.ToListAsync();

            return View(productos);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                return View(producto);
            }
            _db.Productos.Add(producto);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _db.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                return View(producto);
            }
            var productoOriginal = await _db.Productos.FindAsync(producto.Id);

            if(productoOriginal == null)
            {
                return NotFound();
            }

            productoOriginal.Nombre = producto.Nombre;
            productoOriginal.Precio = producto.Precio;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id)
        {

            var producto = await _db.Productos.FindAsync(id);

            if(producto == null)
            {
                return NotFound();
            }
            producto.Activo = !producto.Activo;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");

        }

    }
}
