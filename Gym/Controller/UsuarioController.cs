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
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios() //utilizo ActionREsult <>
        {
            var result = await _usuarioService.GetAllUsers();

            if (result.Success)
            {
                return Ok(result.Data);  // Devuelve HTTP 200 OK con los usuarios
            }

            return BadRequest(new { message = result.ErrorMsg }); // HTTP 400 Bad Request
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUserID(int id)
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
    }
}

