using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shape_app.Controllers;
using shape_app.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace shape_app.Tests
{
    public class ExerciciosControllerTests
    {
        private ExerciciosController _exerciciosController;
        private AppDbContext _appDbContext;

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _appDbContext = new AppDbContext(dbContextOptions);
            _exerciciosController = new ExerciciosController(_appDbContext);
        }

        [TearDown]
        public void Teardown()
		{
			_appDbContext.Dispose();
		}

        [Test]
        public async Task ExerciciosController_GetAll_RetornaVazio_QuandoNaoTemDados()
        {
            var result = await _exerciciosController.GetAll();

			Assert.That((HttpStatusCode)((OkObjectResult)result).StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<List<Exercicio>>(((OkObjectResult)result).Value);
            Assert.AreEqual(0, ((List<Exercicio>)((OkObjectResult)result).Value).Count);
        }

		[Test]
		public async Task ExerciciosController_GetAll_Exercicios_QuandoExisteExerciciosCadastrados()
		{
            // Arrange
            var listaExercicios = new List<Exercicio>
			{
				new Exercicio { Nome = "Supino", Series = 3, Repeticoes = 10 },
				new Exercicio { Nome = "Agachamento", Series = 4, Repeticoes = 12 },
                new Exercicio { Nome = "Rosca direta", Series = 3, Repeticoes = 10 }
			};
            await _appDbContext.Exercicios.AddRangeAsync(listaExercicios);
            await _appDbContext.SaveChangesAsync();

            // Act
			var result = await _exerciciosController.GetAll();

            // Assert
			Assert.That((HttpStatusCode)((OkObjectResult)result).StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.IsInstanceOf<OkObjectResult>(result);
			Assert.IsInstanceOf<List<Exercicio>>(((OkObjectResult)result).Value);
			Assert.AreEqual(listaExercicios.Count, ((List<Exercicio>)((OkObjectResult)result).Value).Count);
		}


        [Test]
		public async Task ExerciciosController_Create_RetornaBadRequest_QuandoSeriesMenorOuIgualAZero()
		{
			var exercicio = new Exercicio { Nome = "Supino", Series = 0, Repeticoes = 10 };

			var result = await _exerciciosController.Create(exercicio);

			Assert.That((HttpStatusCode)((BadRequestObjectResult)result).StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			Assert.IsInstanceOf<ProblemDetails>(((BadRequestObjectResult)result).Value);
			Assert.AreEqual("As séries ou repetições devem ser maiores que 0.", ((ProblemDetails)((BadRequestObjectResult)result).Value).Detail);
		}

		[Test]
		public async Task ExerciciosController_Create_RetornaBadRequest_QuandoRepeticoesMenorOuIgualAZero()
		{
			var exercicio = new Exercicio { Nome = "Supino", Series = 3, Repeticoes = 0 };

			var result = await _exerciciosController.Create(exercicio);

			Assert.That((HttpStatusCode)((BadRequestObjectResult)result).StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			Assert.IsInstanceOf<ProblemDetails>(((BadRequestObjectResult)result).Value);
			Assert.AreEqual("As séries ou repetições devem ser maiores que 0.", ((ProblemDetails)((BadRequestObjectResult)result).Value).Detail);
		}

		[Test]
		public async Task ExerciciosController_Create_RetornaCreated_QuandoExercicioCriadoComSucesso()
		{
			var exercicio = new Exercicio { Nome = "Supino", Series = 3, Repeticoes = 10 };

			var result = await _exerciciosController.Create(exercicio);

			var exerciciosCriados = await _appDbContext.Exercicios.ToListAsync();

			Assert.AreEqual(1, exerciciosCriados.Count);

			Assert.That((HttpStatusCode)((CreatedAtActionResult)result).StatusCode, Is.EqualTo(HttpStatusCode.Created));
			Assert.IsInstanceOf<CreatedAtActionResult>(result);
			Assert.AreEqual("GetById", ((CreatedAtActionResult)result).ActionName);
			Assert.AreEqual(exercicio.Id, ((Exercicio)((CreatedAtActionResult)result).Value).Id);
		}
		[Test]
		public async Task ExerciciosController_GetById_RetornaNotFound_QuandoExercicioNaoExiste()
		{
			var result = await _exerciciosController.GetById(1);

			Assert.That((HttpStatusCode)((NotFoundResult)result).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
			Assert.IsInstanceOf<NotFoundResult>(result);
		}
		[Test]
		public async Task ExerciciosController_GetById_RetornaExercicio_QuandoExercicioExiste()
		{
			var exercicio = new Exercicio { Nome = "Supino", Series = 3, Repeticoes = 10 };
			await _appDbContext.Exercicios.AddAsync(exercicio);
			await _appDbContext.SaveChangesAsync();

			var result = await _exerciciosController.GetById(exercicio.Id);

			Assert.That((HttpStatusCode)((OkObjectResult)result).StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.IsInstanceOf<OkObjectResult>(result);
			Assert.IsInstanceOf<Exercicio>(((OkObjectResult)result).Value);
			Assert.AreEqual(exercicio.Id, ((Exercicio)((OkObjectResult)result).Value).Id);
		}
		[Test]
		public async Task ExerciciosController_Update_RetornaBadRequest_QuandoIdDiferenteDoModel()
		{
			var exercicio = new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 };

			var result = await _exerciciosController.Update(2, exercicio);

			Assert.That((HttpStatusCode)((BadRequestResult)result).StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
			Assert.IsInstanceOf<BadRequestResult>(result);
		}
		[Test]
		public async Task ExerciciosController_Update_RetornaNotFound_QuandoExercicioNaoExiste()
		{
			var exercicio = new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 };

			var result = await _exerciciosController.Update(1, exercicio);

			Assert.That((HttpStatusCode)((NotFoundResult)result).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
			Assert.IsInstanceOf<NotFoundResult>(result);
		}
		[Test]
		public async Task ExerciciosController_Update_RetornaNoContent_QuandoExercicioAtualizadoComSucesso()
		{
			var exercicio = new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 };
			await _appDbContext.Exercicios.AddAsync(exercicio);
			await _appDbContext.SaveChangesAsync();

			exercicio.Nome = "Supino Inclinado";

			var result = await _exerciciosController.Update(1, exercicio);

			var exercicioAtualizado = await _appDbContext.Exercicios.FindAsync(exercicio.Id);

			Assert.That((HttpStatusCode)((NoContentResult)result).StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
			Assert.IsInstanceOf<NoContentResult>(result);
			Assert.AreEqual("Supino Inclinado", exercicioAtualizado.Nome);
		}
		[Test]
		public async Task ExerciciosController_Delete_RetornaNotFound_QuandoExercicioNaoExiste()
		{
			var result = await _exerciciosController.Delete(1);

			Assert.That((HttpStatusCode)((NotFoundResult)result).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
			Assert.IsInstanceOf<NotFoundResult>(result);
		}
		[Test]
		public async Task ExerciciosController_Delete_RetornaNoContent_QuandoExercicioExcluidoComSucesso()
		{
			var exercicio = new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 };
			await _appDbContext.Exercicios.AddAsync(exercicio);
			await _appDbContext.SaveChangesAsync();

			var result = await _exerciciosController.Delete(1);

			var exercicioExcluido = await _appDbContext.Exercicios.FindAsync(exercicio.Id);

			Assert.That((HttpStatusCode)((OkObjectResult)result).StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.IsInstanceOf<OkObjectResult>(result);
			Assert.IsNull(exercicioExcluido);
		}
	}
}
