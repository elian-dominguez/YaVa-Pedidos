using System.ComponentModel.DataAnnotations;

namespace YaVa.Models;

public class CrearUsuarioViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tenés que seleccionar un rol.")]
    public string Rol { get; set; } = string.Empty;
}