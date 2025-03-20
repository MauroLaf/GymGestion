using Gym.Models;
using Microsoft.EntityFrameworkCore;
using Gym.Results;

namespace Gym.Services
{
    public class MembresiaService
    {
        private AppDbContext _context;
        public MembresiaService(AppDbContext context)
        {
            _context = context;
        }
        #region
        // GET ALL
        public async Task<Result<IEnumerable<Membresia>>> GetAllMembresia()
        {
            try
            {
                var membresia = await _context.Membresias.ToListAsync();
                if (membresia == null || membresia.Count == 0)
                {
                    return Result<IEnumerable<Membresia>>.FailureResult("No se encontraron membresias activas");
                }
                return Result<IEnumerable<Membresia>>.SuccessResult(membresia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return Result<IEnumerable<Membresia>>.FailureResult("Ocurrió un error al obtener las membresias");
            }
        }

        // GET ID
        public async Task<Result<Membresia>> GetMembresiaID(int id)
        {
            try
            {
                var membresiaID = await _context.Membresias.FindAsync(id);
                if (membresiaID == null)
                {
                    return Result<Membresia>.FailureResult("No se encontró ninguna membresia asociada al id");
                }
                return Result<Membresia>.SuccessResult(membresiaID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return Result<Membresia>.FailureResult("Ocurrió un error al obtener la membresia");
            }
        }
        //POST
        public async Task<Result<Membresia>> CreateMembresia(Membresia membresia)
        {
            try
            {
                // primero intento agregar a esa lista con el metodo add
                await _context.Membresias.AddAsync(membresia);
                await _context.SaveChangesAsync(); // guardo

                return Result<Membresia>.SuccessResult(membresia); // Retornar la membresia que cree
            }
            catch (DbUpdateException dbEx)
            {
                return Result<Membresia>.FailureResult($"Error al guardar la membresia: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<Membresia>.FailureResult($"Error inesperado al crear membresia: {ex.Message}");
            }
        }
        //PUT
        public async Task<Result<Membresia>> UpdateMembresia(int userId, Membresia membresia)
        {
            try
            {
                // Buscar el usuario por su id
                var usuarioResult = await _context.Usuarios.FindAsync(userId); // Usamos FindAsync si ya tienes el id
                if (usuarioResult == null)
                {
                    // Si no encontramos al usuario, retornamos un mensaje de error
                    return Result<Membresia>.FailureResult("Usuario no encontrado.");
                }

                // Buscar la membresía asociada al usuario a través del MembresiaId
                var membresiaExist = await _context.Membresias.FirstOrDefaultAsync(m => m.Id == usuarioResult.MembresiaId);
                if (membresiaExist == null)
                {
                    // Si no encontramos la membresía asociada, retornamos un mensaje de error
                    return Result<Membresia>.FailureResult("Membresía no encontrada para este usuario.");
                }

                // Actualizamos los campos de la membresía con los valores recibidos
                membresiaExist.Nombre = membresia.Nombre ?? membresiaExist.Nombre;  // Asignamos nombre si es nuevo (si es null, mantenemos el existente)
                membresiaExist.FechaInicio = membresia.FechaInicio;  // Asignamos la fecha de inicio de la membresía
                membresiaExist.Precio = membresia.Precio;  // Actualizamos el precio si es necesario
                membresiaExist.DuracionDias = membresia.DuracionDias;  // Duración de la membresía, si cambia

                // Guardamos los cambios realizados en la base de datos
                await _context.SaveChangesAsync();

                // Retornamos la membresía actualizada
                return Result<Membresia>.SuccessResult(membresiaExist);
            }
            catch (DbUpdateException dbEx)
            {
                // Si ocurre un error al guardar los cambios, retornamos un mensaje de error
                return Result<Membresia>.FailureResult($"Error al actualizar la membresía: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                // Si ocurre cualquier otro error inesperado, retornamos un mensaje genérico de error
                return Result<Membresia>.FailureResult($"Error inesperado al actualizar la membresía: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteMembresia(int id)
        {
            try
            {
                var membresiaExist = await _context.Membresias.FindAsync(id);
                if (membresiaExist == null)
                {
                    return Result<bool>.FailureResult("No se ha encontrado una membresia asociada al id");

                }
                _context.Membresias.Remove(membresiaExist);
                //intento remover y espero nuevamente para guardar los cambios "await"
                await _context.SaveChangesAsync();
                return Result<bool>.SuccessResult(true);
            }
            catch (DbUpdateException dbEx)
            {
                return Result<bool>.FailureResult($"Error en la base de datos al eliminar membresia: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error inesperado al eliminar membresia: {ex.Message}");
            }
        }
        #endregion
        public int ObtenerDuracionPorTipo(string nombre)
        {
            return nombre switch //forma compacta del switch, no se pone case,return,break ni default(_=>)
            {
                "Básica" => 30,
                "Estándar" => 60,
                "Premium" => 90,
                _ => throw new Exception("Tipo de membresia desconocido")
            };
        }

        public bool VencimientoMembresia(Membresia nombreMembresia)
        {
            DateTime fechaVencimiento = nombreMembresia.FechaInicio.AddDays(nombreMembresia.DuracionDias);
            return fechaVencimiento < DateTime.Now; 
        }
        // Método para obtener la membresía de un usuario a partir de su MembresiaId
        public async Task<Result<Membresia>> GetMembresiaPorUsuario(int usuarioId)
        {
            // Buscamos al usuario usando su Id
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId); // Aquí estamos buscando por Id de Usuario

            if (usuario == null)
            {
                // Si no se encuentra el usuario, devolvemos un fallo
                return Result<Membresia>.FailureResult("No se encontró el usuario especificado");
            }

            // Ahora, obtenemos la membresía usando el MembresiaId del usuario
            var membresia = await _context.Membresias
                .FirstOrDefaultAsync(m => m.Id == usuario.MembresiaId);

            if (membresia == null)
            {
                // Si no se encuentra la membresía, devolvemos un fallo
                return Result<Membresia>.FailureResult("No se encontró la membresía asociada al usuario");
            }

            // Si encontramos la membresía, devolvemos éxito
            return Result<Membresia>.SuccessResult(membresia);
        }

    }
}

