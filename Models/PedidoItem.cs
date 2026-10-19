using Microsoft.EntityFrameworkCore;

namespace YaVa.Models;

public class PedidoItem
{
    public int Id { get; set; }

    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    public int Cantidad { get; set; }

    [Precision(10, 2)]
    public decimal PrecioUnitario { get; set; }
}
