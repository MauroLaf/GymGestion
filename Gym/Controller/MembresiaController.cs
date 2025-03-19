using Gym.Models;
using Gym.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Controller
{
    [Route("sistema/[controller]")]
    [ApiController]

    //RECORDAR USAR ActionResult para controlar errores http
    public class MembresiaController : ControllerBase
    {
        private MembresiaService _membresiaService;
        public MembresiaController(MembresiaService membresiaService)
        {
            _membresiaService = membresiaService;
        }
        #region
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
            return NotFound
                (
                new { message = result.ErrorMsg }
                );

        }
        [HttpPost]
        public async Task<ActionResult<Membresia>> AddMembresia([FromBody] Membresia membresia)
        {
            if (membresia == null)
            {
                //retorno un json
                return BadRequest(new { message = "La membresia no puede tener valor nulo" });
            }
            var result = await _membresiaService.CreateMembresia(membresia); //almaceno en resultado la creacion de una membresia
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetMembresiaByID), new { id = result.Data.Id }, result.Data); //recomendacion de gpt para dar mas info
            }
            return BadRequest(new { message = result.ErrorMsg });
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Membresia>> UpdateMembresiaByID(int id, [FromBody] Membresia membresia)

        {
            if (membresia == null)
            {
                return BadRequest(new { message = "La membresia no puede tener valor nulo" });
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
                return result.ErrorMsg == "No se encontró membresia asociada al id"
                    ? NotFound(new { message = result.ErrorMsg }) // 404 si no existe
                    : BadRequest(new { message = result.ErrorMsg }); // 400 si hay otro error
            }

            return Ok(new { message = "Membresia eliminada correctamente" });


        }
        #endregion
    }

}