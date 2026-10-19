using System.ComponentModel.DataAnnotations;

namespace YaVa.Models;

public class NuevoPedidoViewModel
{
    [Required(ErrorMessage = "Indicá la mesa")]
    [StringLength(20)]
    [Display(Name = "Mesa")]
    public string Mesa { get; set; } = "";

    public List<LineaViewModel> Lineas { get; set; } = new();
}

public class LineaViewModel
{
    public int ProductoId { get; set; }

    public string Nombre { get; set; } = "";

    public decimal Precio { get; set; }

    [Range(0, 50, ErrorMessage = "La cantidad tiene que estar entre 0 y 50")]
    public int Cantidad { get; set; }
}
