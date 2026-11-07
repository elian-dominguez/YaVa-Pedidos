
using System.ComponentModel.DataAnnotations;

namespace YaVa.Models;

public class EditarUsuarioViewModel
{
    public string Id { get; set; } = string.Empty;

    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccioná un puesto.")]
    public string Rol { get; set; } = string.Empty;
}
