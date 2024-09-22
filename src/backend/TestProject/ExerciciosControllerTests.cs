using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shape_app.Controllers;
using shape_app.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace shape_app.Tests
{
    public class ExerciciosControllerTests
    {
        private ExerciciosController _controller;
        private AppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new AppDbContext(options);
            _controller = new ExerciciosController(_context);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithListOfExercicios()
        {
            // Arrange
            var exercicios = new List<Exercicio>
            {
                new Exercicio { Id = 1, Nome = "Exercício 1" ,Series = 3, Repeticoes = 10 },
                new Exercicio { Id = 2, Nome = "Exercício 2" ,Series = 4, Repeticoes = 15 }
            };

            _context.Exercicios.AddRange(exercicios);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null");
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(((List<Exercicio>)okResult.Value!).Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Create_ValidExercicio_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var novoExercicio = new Exercicio { Id = 1, Nome = "Exercício 1", Series = 3, Repeticoes = 10 };

            // Act
            var result = await _controller.Create(novoExercicio);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult, "Expected CreatedAtActionResult but got null");
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
        }

        [Test]
        public async Task GetById_ExistingId_ReturnsOkResult_WithExercicio()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            using (var context = new AppDbContext(options))
            {
                var exercicio = new Exercicio { Id = 1, Nome = "Exercício 1", Series = 3, Repeticoes = 10 };
                context.Exercicios.Add(exercicio);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new AppDbContext(options)) 
            {
                var controller = new ExerciciosController(context);
                var result = await controller.GetById(1);

                // Assert
                var okResult = result as OkObjectResult;
                Assert.IsNotNull(okResult);
                Assert.AreEqual(200, okResult.StatusCode);
                Assert.IsInstanceOf<Exercicio>(okResult.Value);
                var returnedExercicio = okResult.Value as Exercicio;
                Assert.AreEqual(1, returnedExercicio.Id);
                Assert.AreEqual("Exercício 1", returnedExercicio.Nome);
            }
        }


        [Test]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            using (var context = new AppDbContext(options))
            {
            }

            // Act
            using (var context = new AppDbContext(options))
            {
                var controller = new ExerciciosController(context);
                var result = await controller.GetById(99); 

                // Assert
                Assert.IsInstanceOf<NotFoundResult>(result);
            }
        }


        [Test]
        public async Task Update_ValidExercicio_ReturnsNoContent()
        {
            // Arrange
            var exercicioExistente = new Exercicio { Id = 1, Nome = "Exercício 1", Series = 3, Repeticoes = 10 };
            var exercicios = new List<Exercicio> { exercicioExistente };

            var updatedExercicio = new Exercicio { Id = 1, Nome = "Exercício 1", Series = 4, Repeticoes = 12 };

            // Act
            var result = await _controller.Update(1, updatedExercicio);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task Update_NonMatchingId_ReturnsBadRequest()
        {
            // Arrange
            var exercicioExistente = new Exercicio { Id = 1, Nome = "Exercício 1", Series = 3, Repeticoes = 10 };
            var exercicios = new List<Exercicio> { exercicioExistente };

            var updatedExercicio = new Exercicio { Id = 2, Nome = "Exercício 2", Series = 4, Repeticoes = 12 }; 

            // Act
            var result = await _controller.Update(1, updatedExercicio);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Delete_ExistingId_ReturnsOk()
        {
            // Arrange
            var exercicioExistente = new Exercicio { Id = 1, Series = 3, Repeticoes = 10 };
            var exercicios = new List<Exercicio> { exercicioExistente };;

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null");
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Delete_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var exercicios = new List<Exercicio>(); 

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }



    }
}
