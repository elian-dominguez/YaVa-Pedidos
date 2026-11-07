using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace YaVa.Services
{
    public class UsuarioService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<IdentityUser>> ObtenerUsuariosAsync()
        {

            return await _userManager.Users.ToListAsync(); ;
        }

        public async Task<IList<string>> ObtenerRolesAsync(IdentityUser usuario)
        {
            return await _userManager.GetRolesAsync(usuario);
        }

        public async Task<List<IdentityRole>> ObtenerRolesDisponiblesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }
        public async Task<IdentityResult> CrearUsuarioAsync(
            string nombreUsuario,
            string password,
            string rol)
        {

            if (rol != "Caja" && rol != "Cocina" && rol != "Mostrador")
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "El rol seleccionado no es válido."
                    });
            }

            var usuario = new IdentityUser
            {
                UserName = nombreUsuario
            };

            var resultado = await _userManager.CreateAsync(usuario, password);

            if (!resultado.Succeeded)
            {
                return resultado;
            }

            var resultadoRol = await _userManager.AddToRoleAsync(usuario, rol);

            if (!resultadoRol.Succeeded)
            {
                var resultadoEliminar = await _userManager.DeleteAsync(usuario);

                if (!resultadoEliminar.Succeeded)
                {
                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "No se pudo asignar el rol ni eliminar el usuario."
                        });
                }

                return resultadoRol;
            }

            return resultadoRol;

        }
        public async Task<IdentityUser?> ObtenerUsuarioPorIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IdentityResult> CambiarRolAsync(
            string usuarioId,
            string rolNuevo)
        {
            if (rolNuevo != "Caja" && rolNuevo != "Cocina" && rolNuevo != "Mostrador")
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "El rol seleccionado no es válido."
                });
            }

            var usuario = await ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuario == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "El usuario no existe."
                });
            }

            var rolesActuales = await _userManager.GetRolesAsync(usuario);

            if (rolesActuales.Contains("Administrador"))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "No se puede modificar el rol de un administrador."
                });
            }

            if (rolesActuales.Count == 1 && rolesActuales.Contains(rolNuevo))
            {
                return IdentityResult.Success;
            }

            if (rolesActuales.Count > 0)
            {
                var resultadoQuitar = await _userManager.RemoveFromRolesAsync(
                    usuario, rolesActuales);

                if (!resultadoQuitar.Succeeded)
                {
                    return resultadoQuitar;
                }
            }

            var resultadoAgregar = await _userManager.AddToRoleAsync(
                usuario, rolNuevo);

            if (!resultadoAgregar.Succeeded && rolesActuales.Count > 0)
            {
                var restauracion = await _userManager.AddToRolesAsync(
                    usuario, rolesActuales);

                if (!restauracion.Succeeded)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "No se pudo asignar el nuevo rol ni restaurar los anteriores. Requiere revisión."
                    });
                }
            }

            return resultadoAgregar;
        }
        public async Task<IdentityResult> DesactivarUsuarioAsync(string usuarioId)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId);

            if (usuario == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "El usuario no existe."
                });
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            if (roles.Contains("Administrador"))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "No se puede desactivar un administrador."
                });
            }

            var resultado = await _userManager.SetLockoutEnabledAsync(usuario, true);

            if (!resultado.Succeeded)
            {
                return resultado;
            }

            return await _userManager.SetLockoutEndDateAsync(
                usuario, DateTimeOffset.MaxValue);
        }
        public async Task<IdentityResult> ReactivarUsuarioAsync(string usuarioId)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId);

            if (usuario == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "El usuario no existe."
                });
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            if (roles.Contains("Administrador"))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "No se puede modificar el estado de un administrador."
                });
            }

            return await _userManager.SetLockoutEndDateAsync(usuario, null);
        }
        public async Task<bool> EstaDesactivadoAsync(IdentityUser usuario)
        {
            return await _userManager.IsLockedOutAsync(usuario);
        }

    }
}
