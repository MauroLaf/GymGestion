using Gym.Models;
using Gym.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Controller
{
    [Route("sistema/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetTotalUsers() //utilizo ActionREsult <>
        {
            var result = await _usuarioService.GetAllUsers();

            if (result.Success)
            {
                return Ok(result.Data);  // Devuelve HTTP 200 OK con los usuarios
            }

            return BadRequest(new { message = result.ErrorMsg }); // HTTP 400 Bad Request
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUserByID(int id) //PRESTAR ATENCION CON EL NOMBRE DEL METODO DEL CONTROLADOR CON EL METODO DEL SERVICIO
        {
            var result = await _usuarioService.GetUserID(id);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            //puedo poner notfound(result.errormsg) pero saldria sin formato json en un simple string
            return NotFound
                (new
                {
                    message = result.ErrorMsg
                }
                );
        }
        [HttpPost]
        public async Task<ActionResult<Usuario>> AddUser([FromBody] Usuario usuario)
        {
            if (usuario == null)
            {
                return BadRequest(new { message = "El usuario no puede ser nulo." });
            }

            var result = await _usuarioService.CreateUser(usuario);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetUserByID), new { id = result.Data.Id }, result.Data); // Retorna HTTP 201 Created
            }
            /*-------------------------------------
            -- ESTO HICE INICIALMENTE MUY BASICO--
             if (result.Success)
           {
               return Ok(result.Data);}
           -------------------------------------*/

            return BadRequest(new { message = result.ErrorMsg }); // HTTP 400 si hay un error en la lógica de negocio
        }
        //POST BATCH (VARIAS CREACIONES)
        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<Usuario>>> AddUsersBatch([FromBody] IEnumerable<Usuario> usuarios)
        {
            var result = new List<Usuario>(); // Lista para almacenar los usuarios creados

            if (usuarios == null || !usuarios.Any())
            {
                return BadRequest(new { message = "La lista de usuarios no puede estar vacía ni ser nula." });
            }

            foreach (var usuario in usuarios)
            {
                var creationResult = await _usuarioService.CreateUser(usuario); // Llamamos al método para crear el usuario

                if (creationResult.Success)
                {
                    result.Add(creationResult.Data); // Si se crea correctamente, lo agregamos a la lista
                }
                else
                {
                    return BadRequest(new { message = creationResult.ErrorMsg }); // Si hay error, devolvemos el mensaje de error
                }
            }

            return Ok(result); // Devuelve la lista de usuarios creados (HTTP 200 OK)
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Usuario>> UpdateUserByID(int id, [FromBody] Usuario usuario)
        {
            if (usuario == null)
            {
                return BadRequest(new { message = "El usuario no puede ser nulo." });
            }
            var result = await _usuarioService.UpdateUser(id, usuario);

            if (result.Success)
            {
                return Ok(result.Data);
            }

            return NotFound(new { message = result.ErrorMsg });
        }

    }
}

