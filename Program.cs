using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YaVa.Data;
using YaVa.Hubs;
using YaVa.Services;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/Login";
    options.AccessDeniedPath = "/Cuenta/AccesoDenegado";

    options.Events.OnValidatePrincipal = async context =>
    {
        var userManager = context.HttpContext.RequestServices
            .GetRequiredService<UserManager<IdentityUser>>();

        var usuario = await userManager.GetUserAsync(context.Principal);

        if (usuario == null ||
            await userManager.IsLockedOutAsync(usuario))
        {
            context.RejectPrincipal();

            await context.HttpContext.SignOutAsync(
                IdentityConstants.ApplicationScheme);
        }
    };
});


builder.Services.AddSignalR();
builder.Services.AddScoped<AvisadorPedidos>();
builder.Services.AddScoped<UsuarioService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
if (app.Environment.IsDevelopment())
    app.UsarUsuarioDePrueba();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<PedidosHub>("/hubs/pedidos");


using (var scope = app.Services.CreateScope())
{
    await SeedDatos.InicializarAsync(scope.ServiceProvider);

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    await SeedRoles.InicializarAsync(roleManager);

    if (app.Environment.IsDevelopment())
    {
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        await SeedAdmin.InicializarAsync(
            userManager,
            app.Configuration);

        await SeedUsuariosPrueba.InicializarAsync(
            userManager,
            app.Configuration);
    }
}


app.Run();