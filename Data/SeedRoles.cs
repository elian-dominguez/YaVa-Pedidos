
using Microsoft.AspNetCore.Identity;

namespace YaVa.Data;

public static class SeedRoles
{
    public static async Task InicializarAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Administrador", "Caja", "Cocina", "Mostrador" };

        foreach (string rol in roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                var resultado = await roleManager.CreateAsync(new IdentityRole(rol));

                if (!resultado.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"No se pudo crear el rol {rol}: " +
                        string.Join(", ", resultado.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
