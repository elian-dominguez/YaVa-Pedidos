using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YaVa.Models;

namespace YaVa.Controllers
{
    public class CuentaController : Controller
    {
        private readonly SignInManager<IdentityUser> _gestorSesiones;

        public CuentaController(SignInManager<IdentityUser> gestorSesiones)
        {
            _gestorSesiones = gestorSesiones;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var resultado = await _gestorSesiones.PasswordSignInAsync(model.Usuario, model.Password, false, false);

            if (!resultado.Succeeded)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View();
            }

            return RedirectToAction("Index", "Home");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _gestorSesiones.SignOutAsync();

            return RedirectToAction("Login", "Cuenta");

        }

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

    }
}
