using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace YaVa.Models;

public class Producto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(80)]
    public string Nombre { get; set; } = "";

    [Precision(10, 2)]
    [Range(0, 999999, ErrorMessage = "El precio no puede ser negativo")]
    public decimal Precio { get; set; }
}
