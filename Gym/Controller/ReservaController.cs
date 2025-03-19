using Gym.Models;
using Gym.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Controller
{

    [Route("sistema/[controller]")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        private ReservaService _reservaService;

        // Constructor donde se inyecta el servicio
        public ReservaController(ReservaService reservaService)
        {
            _reservaService = reservaService;
        }

        // Obtener todas las reservas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetTotalClasses()
        {
            var result = await _reservaService.GetAllReservas(); // Llamada al servicio que obtiene todas las reservas
            if (result.Success)
            {
                return Ok(result.Data); // Si tiene éxito, devuelve las reservas
            }
            return BadRequest(new { message = result.ErrorMsg }); // Si no, devuelve el mensaje de error
        }

        // Obtener una reserva por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> GetReservaByID(int id)
        {
            var result = await _reservaService.GetReservaID(id); // Llamada al servicio para obtener reserva por ID
            if (result.Success)
            {
                return Ok(result.Data); // Si tiene éxito, devuelve la reserva encontrada
            }
            return NotFound(new { message = result.ErrorMsg }); // Si no se encuentra la reserva, devuelve mensaje de error
        }

        // Crear una reserva
        [HttpPost("{usuarioId}/{claseId}")]
        public async Task<ActionResult<Reserva>> AddReserva(int usuarioId, int claseId)
        {
            // Llamamos al servicio para crear la reserva
            var result = await _reservaService.CreateReserva(usuarioId, claseId);

            // Si no se ha podido crear la reserva, devolvemos un error
            if (!result.Success)
            {
                return BadRequest(new { message = result.ErrorMsg }); // Retorna el mensaje de error
            }

            // Si la reserva se creó correctamente, devolvemos el resultado con el código 201 (creado)
            return CreatedAtAction(nameof(GetReservaByID), new { id = result.Data.Id }, result.Data); // Devuelve el Id de la reserva recién creada
        }
    }
}