using Gym.Models;
using Microsoft.EntityFrameworkCore;
using Gym.Results;
using Microsoft.Identity.Client;

namespace Gym.Services
{
    public class UsuarioService
    {
        private AppDbContext _context;
        public UsuarioService(AppDbContext context) {
            _context = context;
        }

        //Metodo Get All
        //agregue Result<> a la coleccion
        public async Task<Result<IEnumerable<Usuario>>> GetAllUsers() {
            try
            {
                var usuarios = await _context.Usuarios.ToListAsync(); //aca uso await porque llamo al metodo async y devolvera una lista async con los usuarios

                if (usuarios == null || usuarios.Count == 0)
                {
                    return Result<IEnumerable<Usuario>>.FailureResult("No se encontraron usuarios."); //accedemos a su metodo failure
                }

                return Result<IEnumerable<Usuario>>.SuccesResult(usuarios); //accedemos a su metodo succes
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Error en la base de datos: {dbEx.Message}"); //el msj da info de la exeption (predet x clase exception)
                return Result<IEnumerable<Usuario>>.FailureResult($"Error en la base de datos: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return Result<IEnumerable<Usuario>>.FailureResult($"Error desconocido: {ex.Message}");
            }

        }

        //Metodo Get por ID(int)
        public async Task<Result<Usuario>> GetUserID(int id)
        {
            try { 
        var usuarioID = await _context.Usuarios.FindAsync(id);
            if(usuarioID == null)
                {
                    return Result<Usuario>.FailureResult("No se encuentra el usuario con ese id");
                }
                return Result<Usuario>.SuccesResult(usuarioID);
            }
            catch (DbUpdateException dbEx) {
                Console.WriteLine($"Error en la base de datos: {dbEx.Message}");
                return Result<Usuario>.FailureResult($"Error en la base de datos: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return Result<Usuario>.FailureResult($"Error desconocido: {ex.Message}");
            }
        

        }
    }
}
