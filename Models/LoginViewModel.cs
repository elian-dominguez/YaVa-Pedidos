using System.ComponentModel.DataAnnotations;

namespace YaVa.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingresá tu usuario")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá tu contraseña")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}