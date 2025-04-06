using Gym.Models;
using Gym.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura el DbContext con db

var connectionString = builder.Configuration.GetConnectionString("defaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.23")));

// 1. Servicios
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect("defaultConnection")));

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

