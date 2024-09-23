using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shape_app.Models;

namespace shape_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciciosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExerciciosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Exercicios.ToListAsync();
            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Exercicio model)
        {
            if(model.Series <= 0 || model.Repeticoes <= 0)
            {
                return BadRequest(new ProblemDetails { Detail = "As séries ou repetições devem ser maiores que 0."});
            }

            _context.Exercicios.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetById", new {id = model.Id}, model);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Exercicios
                .FirstOrDefaultAsync(c => c.Id == id);

            if (model == null)
            {
                return NotFound();
            }


            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Exercicio model)
        {
            if (id != model.Id) return BadRequest();

            var modeloDb = await _context.Exercicios
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (modeloDb == null) return NotFound();

            _context.Exercicios.Update(model);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _context.Exercicios
                .FirstOrDefaultAsync(c => c.Id == id);

            if (model == null) {                 
                return NotFound();
            };

            _context.Exercicios.Remove(model);
            await _context.SaveChangesAsync();

            return Ok("Exercício excluído com sucesso");
        }

    }
}