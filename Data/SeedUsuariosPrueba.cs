
using Microsoft.AspNetCore.Identity;

namespace YaVa.Data;

public static class SeedUsuariosPrueba
{
    public static async Task InicializarAsync(
        UserManager<IdentityUser> userManager,
        IConfiguration configuration)
    {
        string[] roles = { "Caja", "Cocina", "Mostrador" };

        foreach (var rol in roles)
        {
            string? usuario = configuration[
                $"SeedUsuariosPrueba:{rol}:UserName"];

            string? password = configuration[
                $"SeedUsuariosPrueba:{rol}:Password"];

            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    $"Faltan credenciales para {rol}.");
            }

            var empleado = await userManager.FindByNameAsync(usuario);

            if (empleado == null)
            {
                empleado = new IdentityUser
                {
                    UserName = usuario
                };

                var resultado = await userManager.CreateAsync(
                    empleado, password);

                if (!resultado.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(", ",
                            resultado.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(empleado, rol))
            {
                var resultado = await userManager.AddToRoleAsync(
                    empleado, rol);

                if (!resultado.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(", ",
                            resultado.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
