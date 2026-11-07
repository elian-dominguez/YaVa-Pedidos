using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YaVa.Models;
using YaVa.Services;

namespace YaVa.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {

        private readonly UsuarioService _usuarioService;

        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioService.ObtenerUsuariosAsync();

            var listado = new List<UsuarioListadoViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _usuarioService.ObtenerRolesAsync(usuario);

                var estaDesactivado =
                    await _usuarioService.EstaDesactivadoAsync(usuario);

                listado.Add(new UsuarioListadoViewModel
                {
                    Id = usuario.Id,
                    NombreUsuario = usuario.UserName ?? "",
                    Rol = roles.FirstOrDefault() ?? "",
                    EstaDesactivado = estaDesactivado
                });
            }

            return View(listado);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CrearUsuarioViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var resultado = await _usuarioService.CrearUsuarioAsync(
                model.NombreUsuario,
                model.Password,
                model.Rol
            );

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            TempData["MensajeExito"] = "Empleado creado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            var roles = await _usuarioService.ObtenerRolesAsync(usuario);

            if (roles.Contains("Administrador"))
            {
                return Forbid();
            }

            var model = new EditarUsuarioViewModel
            {
                Id = usuario.Id,
                NombreUsuario = usuario.UserName ?? "",
                Rol = roles.FirstOrDefault() ?? ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditarUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var resultado = await _usuarioService.CambiarRolAsync(
                model.Id,
                model.Rol
            );

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            TempData["MensajeExito"] = "Rol actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(string id)
        {
            var resultado = await _usuarioService.DesactivarUsuarioAsync(id);

            if (resultado.Succeeded)
            {
                TempData["MensajeExito"] = "Empleado desactivado correctamente.";
            }
            else
            {
                TempData["MensajeError"] = string.Join(" ",
                    resultado.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivar(string id)
        {
            var resultado = await _usuarioService.ReactivarUsuarioAsync(id);

            if (resultado.Succeeded)
            {
                TempData["MensajeExito"] = "Empleado reactivado correctamente.";
            }
            else
            {
                TempData["MensajeError"] = string.Join(" ",
                    resultado.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
