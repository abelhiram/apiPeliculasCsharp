using ApPeliculas.Data;
using ApPeliculas.Modelos;
using ApPeliculas.Modelos.Dtos;
using ApPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace ApPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _db;
        private string claveSecreta;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UsuarioRepositorio(ApplicationDbContext db,IConfiguration config,
            RoleManager<IdentityRole> roleManager, UserManager<AppUsuario> userManager,IMapper mapper)
        {
            _db = db;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public AppUsuario GetUsuario(string usuarioId)
        {
            return _db.AppUsuario.FirstOrDefault(u => u.Id == usuarioId);
        }

        public ICollection<AppUsuario> GetUsuarios()
        {
            return _db.AppUsuario.OrderBy(u => u.UserName).ToList();
        }

        public bool IsUniqueUser(string usuario)
        {
            var usuarioDb = _db.AppUsuario.FirstOrDefault(u => u.UserName == usuario);
            if (usuarioDb == null)
            {
                return true;
            }
            return false;
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            //var passwordEncriptado = obtenermd5(usuarioLoginDto.Password);

            var usuario = _db.AppUsuario.FirstOrDefault(
                u => u.UserName.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
                //&& u.Password == passwordEncriptado
                );
            bool isValida = await _userManager.CheckPasswordAsync(usuario, usuarioLoginDto.Password);
            ///////
            if (usuario == null || isValida == false) 
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            ////////existe el usuario y podemos procesar el login
            ///
            var roles  = await _userManager.GetRolesAsync(usuario);
            var manejadoroken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDecrypt = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };

            var token = manejadoroken.CreateToken(tokenDecrypt);

            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                Token = manejadoroken.WriteToken(token),
                Usuario = _mapper.Map<UsuarioDatosDto>(usuario)

            };
            return usuarioLoginRespuestaDto;
        }

        public async Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            //var passwordEncriptado = obtenermd5(usuarioRegistroDto.Password);

            AppUsuario usuario = new AppUsuario()
            {
                UserName = usuarioRegistroDto.NombreUsuario,
                Email = usuarioRegistroDto.NombreUsuario,
                NormalizedEmail = usuarioRegistroDto.NombreUsuario.ToUpper(),
                Nombre = usuarioRegistroDto.Nombre
            };
            var result = await _userManager.CreateAsync(usuario, usuarioRegistroDto.Password);
            if (result.Succeeded)
            {
                //solo la primera vez y para crear los roles
                if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    await _roleManager.CreateAsync(new IdentityRole("registrado"));
                }
            
                await _userManager.AddToRoleAsync(usuario, "admin");
                var usuarioRetornado = _db.AppUsuario.FirstOrDefault(u => u.UserName == usuarioRegistroDto.NombreUsuario);
                //opcion 1
                //return new UsuarioDatosDto()
                //{
                //    Id = usuarioRetornado.Id,
                //    UserName = usuarioRetornado.UserName,
                //    Nombre = usuarioRetornado.Nombre
                //};

                //_db.Usuario.Add(usuario);
                //await _db.SaveChangesAsync();
                //usuario.Password = passwordEncriptado;
                //return usuario;
                return _mapper.Map<UsuarioDatosDto>(usuarioRetornado);
            }
            return new UsuarioDatosDto();
        }

        public static string obtenermd5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);

            data = x.ComputeHash(data);
            string resp = "";

            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;
        }

        
    }
}
