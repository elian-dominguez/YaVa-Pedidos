
using Microsoft.AspNetCore.Identity;

namespace YaVa.Data
{
    public static class SeedAdmin
    {
        public static async Task InicializarAsync(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            string? usuario = configuration["SeedAdmin:UserName"];
            string? password = configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            var admin = await userManager.FindByNameAsync(usuario);

            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = usuario
                };

                var resultado = await userManager.CreateAsync(admin, password);

                if (!resultado.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(", ",
                            resultado.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "Administrador"))
            {
                var resultado = await userManager.AddToRoleAsync(
                    admin, "Administrador");

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
