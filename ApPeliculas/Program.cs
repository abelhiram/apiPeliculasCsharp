using ApPeliculas.Data;
using ApPeliculas.Repositorio;
using ApPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApPeliculas.PeliculasMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ApPeliculas.Modelos;

var builder = WebApplication.CreateBuilder(args);
//Configuramos la conexion a sqlserver

builder.Services.AddDbContext<ApplicationDbContext>(Opciones =>
{
    Opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSQL"));
});

//autenticacion .net identity

builder.Services.AddIdentity<AppUsuario, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

//añadir cache
builder.Services.AddResponseCaching();

//agregar repositorios
builder.Services.AddScoped<ICategoriaRepositorio,CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");
// agregar auto mapper

builder.Services.AddAutoMapper(typeof(PeliculasMapper));

//aqui se configura la autenticacion
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
}); 


// Add services to the container.

builder.Services.AddControllers(opcion =>
{
    opcion.CacheProfiles.Add("PorDefecto20sec", new CacheProfile() { Duration = 20});
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticacion JWT \n"+
        "ingresa  Bearer seguida de un espacio y despues su token: \n",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"

    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


//soporte para cors
builder.Services.AddCors(p => p.AddPolicy("PolicyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//soporte para cors

app.UseCors("PolicyCors");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
