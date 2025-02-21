namespace L01_2018AC605.Models
{
    public class Comentario
    {
        public int ComentarioId { get; set; }
        public int PublicacionId { get; set; }
        public string Contenido { get; set; }
        public int UsuarioId { get; set; }
    }
}
