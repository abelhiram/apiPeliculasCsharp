using ApPeliculas.Modelos;
using ApPeliculas.Modelos.Dtos;

namespace ApPeliculas.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
        ICollection<AppUsuario> GetUsuarios();

        AppUsuario GetUsuario(string usuarioId);

        bool IsUniqueUser(string usuario);
        Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto);
        //Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto);
        Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto);

    }
}
