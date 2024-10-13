using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shape_app.Models;

namespace shape_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreinosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TreinosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Treinos.ToListAsync();
            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Treino model)
        {

            _context.Treinos.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetById", new { id = model.Id }, model);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Treinos
               .Include(t => t.Exercicios).ThenInclude(t => t.Exercicio)
               .FirstOrDefaultAsync(c => c.Id == id);

            if (model == null)
            {
                return NotFound();
            }


            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Treino model)
        {
            if (id != model.Id) return BadRequest();

            var modeloDb = await _context.Treinos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (modeloDb == null) return NotFound();

            _context.Treinos.Update(model);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _context.Treinos
                .FirstOrDefaultAsync(c => c.Id == id);

            if (model == null)
            {
                return NotFound();
            };

            _context.Treinos.Remove(model);
            await _context.SaveChangesAsync();

            return Ok("Treino excluído com sucesso");
        }






        [HttpPost("{id}/exercicios")]
        public async Task<IActionResult> AddExercicio(int id, TreinoExercicio model)
        {
            if(id != model.TreinoId) return BadRequest();

            _context.TreinoExercicios.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetById", new { id = model.TreinoId }, model);
        }






        [HttpDelete("{id}/exercicios/{exercicioId}")]
        public async Task<IActionResult> DeleteExercicio(int id, int exercicioId)
        {
            var model = await _context.TreinoExercicios
                .Where(c => c.TreinoId == id && c.ExercicioId == exercicioId )
                .FirstOrDefaultAsync();

            if (model == null) return NotFound();

            _context.TreinoExercicios.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
