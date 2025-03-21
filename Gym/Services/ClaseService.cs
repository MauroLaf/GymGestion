using Gym.Models;
using Microsoft.EntityFrameworkCore;
using Gym.Results;

namespace Gym.Services
{
    public class ClaseService
    {
        private AppDbContext _context;

        public ClaseService(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<Result<IEnumerable<Clase>>> GetAllClasses()
        {
            try
            {
                var clases = await _context.Clases.ToListAsync();
                if (clases == null || clases.Count == 0)
                {
                    return Result<IEnumerable<Clase>>.FailureResult("No se encontraron clases.");
                }
                return Result<IEnumerable<Clase>>.SuccessResult(clases);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return Result<IEnumerable<Clase>>.FailureResult("Ocurrió un error al obtener las clases.");
            }
        }

        // GET ID
        public async Task<Result<Clase>> GetClassID(int id)
        {
            try
            {
                var claseID = await _context.Clases.FindAsync(id);
                if (claseID == null)
                {
                    return Result<Clase>.FailureResult("No se encontró ninguna clase con el ID especificado.");
                }
                return Result<Clase>.SuccessResult(claseID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return Result<Clase>.FailureResult("Ocurrió un error al obtener la clase.");
            }
        }
        //POST
        public async Task<Result<Clase>> CreateClass(Clase clase)
        {
            try
            {
                // primero intento agregar a esa lista con el metodo add
                await _context.Clases.AddAsync(clase);
                await _context.SaveChangesAsync(); // guardo

                return Result<Clase>.SuccessResult(clase); // Retornar la clase que cree
            }
            catch (DbUpdateException dbEx)
            {
                return Result<Clase>.FailureResult($"Error al guardar la clase: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<Clase>.FailureResult($"Error inesperado al crear la clase: {ex.Message}");
            }
        }
        //PUT
        public async Task<Result<Clase>> UpdateClass(int id, Clase clase)
        {
            try
            {
                var classExist = await _context.Clases.FindAsync(id);
                if (classExist == null)
                {
                    //retornamos un result que es el tipo que debemos devolver y accedemos a su metodo para manejar error
                    return Result<Clase>.FailureResult("No se ha encontrado una clase con ese id");
                }
                
                //
                _context.Entry(classExist).CurrentValues.SetValues(clase);
                
                await _context.SaveChangesAsync();

                return Result<Clase>.SuccessResult(classExist);
            }
            catch (DbUpdateException dbEx)
            {
                return Result<Clase>.FailureResult($"Error al actualizar el usuario: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<Clase>.FailureResult($"Error inesperado al actualizar el usuario: {ex.Message}");
            }
        }
        public async Task<Result<bool>> DeleteClass(int id)
        {
            try
            {
                var classExist = await _context.Clases.FindAsync(id);
                if (classExist == null)
                {
                    return Result<bool>.FailureResult("No se ha encontrado una clase con ese id");

                }
                _context.Clases.Remove(classExist);
                //intento remover y espero nuevamente para guardar los cambios "await"
                await _context.SaveChangesAsync();
                return Result<bool>.SuccessResult(true);
            }
            catch (DbUpdateException dbEx)
            {
                return Result<bool>.FailureResult($"Error en la base de datos al eliminar usuario: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error inesperado al eliminar usuario: {ex.Message}");
            }
        }
    }
}


