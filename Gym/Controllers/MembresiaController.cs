using Gym.Models;
using Gym.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Controllers
{
    [Route("sistema/[controller]")]
    [ApiController]

    // RECORDAR USAR ActionResult para controlar errores HTTP
    public class MembresiaController : ControllerBase
    {
        private readonly MembresiaService _membresiaService;
        public MembresiaController(MembresiaService membresiaService)
        {
            _membresiaService = membresiaService;
        }

        #region Métodos CRUD

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Membresia>>> GetTotalMembresia()
        {
            var result = await _membresiaService.GetAllMembresia();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(new { message = result.ErrorMsg });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Membresia>> GetMembresiaByID(int id)
        {
            var result = await _membresiaService.GetMembresiaID(id);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return NotFound(new { message = result.ErrorMsg });
        }

        [HttpPost]
        public async Task<ActionResult<Membresia>> AddMembresia([FromBody] Membresia membresia)
        {
            if (membresia == null)
            {
                // Retorno un JSON indicando que la membresía no puede ser nula
                return BadRequest(new { message = "La membresía no puede tener valor nulo" });
            }

            // ULTIMO  Asignar duración basada en el tipo de membresía
            membresia.DuracionDias = _membresiaService.ObtenerDuracionPorTipo(membresia.Nombre);

            var result = await _membresiaService.CreateMembresia(membresia); // Almaceno el resultado de la creación
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetMembresiaByID), new { id = result.Data.Id }, result.Data);
            }
            return BadRequest(new { message = result.ErrorMsg });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Membresia>> UpdateMembresiaByID(int id, [FromBody] Membresia membresia)
        {
            if (membresia == null)
            {
                return BadRequest(new { message = "La membresía no puede tener valor nulo" });
            }
            var result = await _membresiaService.UpdateMembresia(id, membresia);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return NotFound(new { message = result.ErrorMsg });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMembresiaByID(int id)
        {
            var result = await _membresiaService.DeleteMembresia(id);
            if (!result.Success)
            {
                return result.ErrorMsg == "No se encontró membresía asociada al id"
                    ? NotFound(new { message = result.ErrorMsg }) // 404 si no existe
                    : BadRequest(new { message = result.ErrorMsg }); // 400 si hay otro error
            }
            return Ok(new { message = "Membresía eliminada correctamente" });
        }

        #endregion

        #region Métodos Adicionales

        // Endpoint para verificar si una membresía está vencida
        [HttpGet("vencimiento/{id}")]
        public async Task<IActionResult> VerificarVencimiento(int id)
        {
            var result = await _membresiaService.GetMembresiaID(id);
            if (!result.Success)
            {
                return NotFound(new { message = result.ErrorMsg });
            }
            bool vencida = _membresiaService.VencimientoMembresia(result.Data);
            return Ok(new { Id = id, Vencida = vencida });
        }

        // Método para obtener la membresía asociada a un usuario
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<Membresia>> GetMembresiaPorUsuario(int usuarioId)
        {
            var result = await _membresiaService.GetMembresiaPorUsuario(usuarioId);

            if (result.Success)
            {
                return Ok(result.Data); // Retorna la membresía si la búsqueda fue exitosa
            }

            return NotFound(new { message = result.ErrorMsg }); // Si no se encontró, retorna un error
        }


        #endregion
    }
}
