using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YaVa.Models;

public class Pedido
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Indicá la mesa")]
    [StringLength(20)]
    public string Mesa { get; set; } = "";

    public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

    public DateTime CreadoEn { get; set; } = DateTime.Now;

    [StringLength(256)]
    public string CreadoPor { get; set; } = "";

    public List<PedidoItem> Items { get; set; } = new();

    [NotMapped]
    public decimal Total => Items.Sum(i => i.Cantidad * i.PrecioUnitario);
}
