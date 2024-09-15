﻿using Microsoft.AspNetCore.Authorization;
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
                return BadRequest(new {message = "As séries ou repetições devem ser maiores que 0."});
            }

            _context.Exercicios.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }
    }
}