namespace YaVa.Models
{
    public class UsuarioListadoViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;

        public bool EstaDesactivado { get; set; }
    }
}
