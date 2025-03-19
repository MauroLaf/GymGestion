using Gym.Models;
using Microsoft.EntityFrameworkCore;
using Gym.Results;

public class ReservaService
{
    private AppDbContext _context; // Se crea el objeto _context para interactuar con la base de datos

    public ReservaService(AppDbContext context)
    {
        _context = context; // Se inyecta el contexto en el constructor
    }

    // Método para crear una reserva
    public async Task<Result<Reserva>> CreateReserva(int usuarioId, int claseId)
    {
        // Verificamos si el usuario existe
        var usuarioExist = await _context.Usuarios.FindAsync(usuarioId);
        if (usuarioExist == null)
        {
            return Result<Reserva>.FailureResult("Usuario no encontrado"); // Si no existe el usuario, devolvemos error
        }

        // Verificamos si la clase existe
        var claseExist = await _context.Clases.FindAsync(claseId);
        if (claseExist == null)
        {
            return Result<Reserva>.FailureResult("Clase no encontrada"); // Si no existe la clase, devolvemos error
        }

        // Verificamos si hay cupos disponibles en la clase
        if (claseExist.CuposMax == 0)
            return Result<Reserva>.FailureResult("No hay cupos disponibles");

        // Verificamos si ya existe una reserva para este usuario en esta clase
        var reservaExist = await _context.Reservas.FirstOrDefaultAsync(r => r.UsuarioId == usuarioId && r.ClaseId == claseId);
        if (reservaExist != null)
        {
            return Result<Reserva>.FailureResult("Ya tienes una reserva en esta clase."); // Si ya existe, devolvemos error
        }

        // Creamos una nueva reserva
        var reserva = new Reserva
        {
            UsuarioId = usuarioId,
            ClaseId = claseId,
            FechaReserva = DateTime.UtcNow // Asignamos la fecha de la reserva
        };

        // Restamos un cupo en la clase
        claseExist.CuposMax--;

        // Añadimos la nueva reserva a la base de datos
        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync(); // Guardamos los cambios en la base de datos, el Id se genera automáticamente

        // Devolvemos la reserva creada con éxito
        return Result<Reserva>.SuccessResult(reserva); // El Id ya está asignado automáticamente por la base de datos
    }

    // Método para obtener todas las reservas
    public async Task<Result<IEnumerable<Reserva>>> GetAllReservas()
    {
        try
        {
            var reserva = await _context.Reservas.ToListAsync(); // Obtenemos todas las reservas

            // Verificamos si hay reservas
            if (reserva == null || !reserva.Any())
            {
                return Result<IEnumerable<Reserva>>.FailureResult("No se encontraron reservas."); // Si no hay reservas, devolvemos error
            }

            return Result<IEnumerable<Reserva>>.SuccessResult(reserva); // Si hay reservas, devolvemos el resultado
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Reserva>>.FailureResult($"Error al obtener las reservas: {ex.Message}"); // Capturamos el error y lo devolvemos
        }
    }

    // Método para obtener una reserva por ID
    public async Task<Result<Reserva>> GetReservaID(int id)
    {
        try
        {
            var reservaID = await _context.Reservas.FindAsync(id); // Buscamos la reserva por Id
            if (reservaID == null)
            {
                return Result<Reserva>.FailureResult("No se encontró una reserva asociada al id"); // Si no existe la reserva, devolvemos error
            }
            return Result<Reserva>.SuccessResult(reservaID); // Si la reserva existe, la devolvemos
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado: {ex.Message}"); // Mostramos el error en consola
            return Result<Reserva>.FailureResult("Ocurrió un error al obtener la reserva."); // Devolvemos el mensaje de error
        }
    }
}