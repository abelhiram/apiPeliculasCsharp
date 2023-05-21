using System.ComponentModel.DataAnnotations;

namespace ApPeliculas.Modelos
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
    }
}
