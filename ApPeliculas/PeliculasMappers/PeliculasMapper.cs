using ApPeliculas.Modelos;
using ApPeliculas.Modelos.Dtos;
using AutoMapper;

namespace ApPeliculas.PeliculasMapper
{
    public class PeliculasMapper: Profile
    {
        public PeliculasMapper()
        {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Categoria, CrearCategoriaDto>().ReverseMap();
            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<AppUsuario, UsuarioDto>().ReverseMap();
            CreateMap<AppUsuario, UsuarioDatosDto>().ReverseMap();
        }

    }
}
