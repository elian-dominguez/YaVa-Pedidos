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
        
      
    }
}
