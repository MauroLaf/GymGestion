using Gym.Models;
using Microsoft.EntityFrameworkCore;
using Gym.Results;

namespace Gym.Services
{
    public class MembresiaService
    {
        private AppDbContext _context;
        public MembresiaService (AppDbContext context)
        {  
            _context = context; 
        }
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
        public async Task<Result<Membresia>> UpdateMembresia(int id, Membresia membresia)
        {
            try
            {
                var membresiaExist = await _context.Membresias.FindAsync(id);
                if (membresiaExist == null)
                {
                    //retornamos un result que es el tipo que debemos devolver y accedemos a su metodo para manejar error
                    return Result<Membresia>.FailureResult("No se ha encontrado una membresia asociada al id");
                }
                membresiaExist.Nombre = membresia.Nombre ?? membresiaExist.Nombre;
                /*
                VER COMO PONER COALESCE EN int y decimal
                membresiaExist.Precio = membresia.Precio ?? membresiaExist.Precio;
                membresiaExist.DuracionDias = membresia.DuracionDias ?? membresia.DuracionDias;
                */
                await _context.SaveChangesAsync();

                return Result<Membresia>.SuccessResult(membresiaExist);
            }
            catch (DbUpdateException dbEx)
            {
                return Result<Membresia>.FailureResult($"Error al actualizar membresia: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<Membresia>.FailureResult($"Error inesperado al actualizar membresia: {ex.Message}");
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
    }
}
    }
}
