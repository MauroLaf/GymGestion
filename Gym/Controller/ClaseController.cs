using Gym.Models;
using Gym.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Controller
{
    [Route("sistema/[controller]")]
    [ApiController]

    //RECORDAR USAR ActionResult para controlar errores http
    public class ClaseController : ControllerBase
    {
        private ClaseService _claseService;
        public ClaseController(ClaseService claseService)
        {
            _claseService = claseService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clase>>> GetTotalClasses()
        {
            var result = await _claseService.GetAllClasses();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(new { message = result.ErrorMsg });
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Clase>> GetClassByID(int id)
        {
            var result = await _claseService.GetClassID(id);
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
        public async Task<ActionResult<Clase>> AddClass([FromBody]Clase clase)
        {
            if (clase == null)
            {
                //retorno un json
                return BadRequest(new { message = "La clase no puede tener valor nulo" });
            }
            var result = await _claseService.CreateClass(clase); //almaceno en resultado la creacion de una clase
            if (result.Success)
            {  
                return CreatedAtAction(nameof(GetClassByID), new { id = result.Data.Id }, result.Data); //recomendacion de gpt para dar mas info
            }
            return BadRequest(new { message = result.ErrorMsg });
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Clase>> UpdateClassByID([FromBody]int id, Clase clase)
        {
            if (clase == null)
            {
                return BadRequest(new { message = "El usuario no puede ser nulo" });
            }
            var result = await _claseService.UpdateClass(id, clase);
            if (result.Success)
            {  
                return Ok(result.Data); 
            }
            return NotFound(new { message = result.ErrorMsg });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassByID(int id)
        {
            var result = await _claseService.DeleteClass(id);
            if (!result.Success)
            {
                return result.ErrorMsg == "Clase no encontrado por id"
                    ? NotFound(new { message = result.ErrorMsg }) // 404 si no existe
                    : BadRequest(new { message = result.ErrorMsg }); // 400 si hay otro error
            }

            return Ok(new { message = "Clase eliminada correctamente" });
        
    
        }
    }   
}
