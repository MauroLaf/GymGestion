using Gym.Models;
using Gym.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura el DbContext con SQL Server
var connectionString = builder.Configuration.GetConnectionString("defaultConnection");

// 1. Servicios
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 1.2. Registro del servicio UsuarioService
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ClaseService>();
builder.Services.AddScoped<MembresiaService>();
builder.Services.AddScoped<ReservaService>();

// 2. Controladores
builder.Services.AddControllers();


var app = builder.Build();

app.MapControllers();
app.Run();

