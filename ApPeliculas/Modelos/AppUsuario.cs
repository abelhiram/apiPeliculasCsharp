using Microsoft.AspNetCore.Identity;

namespace ApPeliculas.Modelos
{
    public class AppUsuario : IdentityUser
    {
        //añadir campos personalizaados
        public string Nombre { get; set; }


    }
}
