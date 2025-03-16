using Gym.Models;
using Microsoft.EntityFrameworkCore;
using Gym.Results;

namespace Gym.Services
{
    public class UsuarioService
    {
        private AppDbContext _context;
        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        //Metodo Get All
        //agregue Result<> a la coleccion
        public async Task<Result<IEnumerable<Usuario>>> GetAllUsers()
        {
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
            try
            {
                var usuarioID = await _context.Usuarios.FindAsync(id);
                if (usuarioID == null)
                {
                    return Result<Usuario>.FailureResult("No se encuentra el usuario con ese id");
                }
                return Result<Usuario>.SuccesResult(usuarioID);
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Error en la base de datos: {dbEx.Message}");
                return Result<Usuario>.FailureResult($"Error en la base de datos: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return Result<Usuario>.FailureResult($"Error desconocido: {ex.Message}");
            }


        }
        //Metodo Post
        public async Task<Result<Usuario>> CreateUser(Usuario usuario)
        {
            try
            {
                // Agregar el usuario a la base de datos
                await _context.Usuarios.AddAsync(usuario);
                await _context.SaveChangesAsync(); // Guardar los cambios en la base de datos

                return Result<Usuario>.SuccesResult(usuario); // Retornar el usuario creado
            }
            catch (DbUpdateException dbEx)
            {
                return Result<Usuario>.FailureResult($"Error al guardar el usuario: {dbEx.Message}");
            }
        }
        //Metodo Put
        public async Task<Result<Usuario>> UpdateUser(int id, Usuario usuario)
        {
            try
            {
                var userExist = await _context.Usuarios.FindAsync(id);
                if (userExist == null)
                {
                    return Result<Usuario>.FailureResult("Usuario no encontrado");
                }
                // Actualizar los campos recibidos, uso coalesce con usuario existente, si es null queda tal cual y si no es null se actualiza con lo que envie en "usuario"
                userExist.Nombre = usuario.Nombre ?? userExist.Nombre;
                userExist.Email = usuario.Email ?? userExist.Email;
                userExist.Telefono = usuario.Telefono ?? userExist.Telefono;
                userExist.FechaRegistro = usuario.FechaRegistro ?? userExist.FechaRegistro;
                userExist.Rol = usuario.Rol ?? userExist.Rol;
                await _context.SaveChangesAsync();

                return Result<Usuario>.SuccesResult(userExist);
            }
            catch (DbUpdateException dbEx)
            {
                return Result<Usuario>.FailureResult($"Error al actualizar el usuario: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<Usuario>.FailureResult($"Error desconocido: {ex.Message}");
            }
        }
        //Metodo Delete
        //ya que no devuelve el usuario devolvera un tipo bool si existe o no
        public async Task<Result<bool>> DeleteUser(int id)
        {
            try
            {
                var userExist = await _context.Usuarios.FindAsync(id);
                if (userExist == null)
                {
                    return Result<bool>.FailureResult("Usuario no encontrado");
                }
                //accion
                _context.Usuarios.Remove(userExist);
                await _context.SaveChangesAsync();//guardo cambios

                //retornar exito
                return Result<bool>.SuccesResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error al eliminar el usuario: {ex.Message}");
            }

        }
    }
}
