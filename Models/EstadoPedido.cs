namespace YaVa.Models;

public enum EstadoPedido
{
    Pendiente = 0,
    EnPreparacion = 1,
    Listo = 2,
    Entregado = 3
}

public static class EstadoPedidoTexto
{
    public static string Texto(this EstadoPedido estado) => estado switch
    {
        EstadoPedido.Pendiente => "Pendiente",
        EstadoPedido.EnPreparacion => "En preparación",
        EstadoPedido.Listo => "Listo",
        EstadoPedido.Entregado => "Entregado",
        _ => estado.ToString()
    };
}
