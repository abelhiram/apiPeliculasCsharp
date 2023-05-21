using System.ComponentModel.DataAnnotations;

namespace ApPeliculas.Modelos.Dtos
{
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage ="El usuario es obligatorio")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "contraseña obligatoria")]
        public string Password { get; set; }
    }
}
