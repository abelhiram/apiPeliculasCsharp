using System.ComponentModel.DataAnnotations;

namespace ApPeliculas.Modelos.Dtos
{
    public class CrearCategoriaDto
    {

        [Required(ErrorMessage ="El nombre es obligatorio")]
        [MaxLength(60, ErrorMessage = "excede el numero maximo de caracteres")]
        public string Nombre { get; set; }


    }
}
