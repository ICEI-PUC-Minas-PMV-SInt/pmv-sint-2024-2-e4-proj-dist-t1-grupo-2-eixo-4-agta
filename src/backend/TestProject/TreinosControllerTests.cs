using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shape_app.Controllers;
using shape_app.Models;
using System.Net;

namespace TestProject
{
    internal class TreinosControllerTests
    {

        private TreinosController _treinosController;
        private AppDbContext _appDbContext;

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _appDbContext = new AppDbContext(dbContextOptions);
            _treinosController = new TreinosController(_appDbContext);
        }

        [TearDown]
        public void Teardown()
        {
            _appDbContext.Dispose();
        }

        [Test]
        public async Task TreinosController_GetAll_RetornaVazio_QuandoNaoTemDados()
        {
            // Act
            var result = await _treinosController.GetAll();

            // Assert
            Assert.That((HttpStatusCode)((OkObjectResult)result).StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<List<Treino>>(((OkObjectResult)result).Value);
            Assert.AreEqual(0, ((List<Treino>)((OkObjectResult)result).Value).Count);
        }

        [Test]
        public async Task TreinosController_GetAll_QuandoExisteTreinosCadastrados()
        {
            // Arrange
            var listaExercicios = new List<Exercicio>
            {
                new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 },
                new Exercicio { Id = 2, Nome = "Agachamento", Series = 4, Repeticoes = 12 },
                new Exercicio { Id = 3, Nome = "Rosca direta", Series = 3, Repeticoes = 10 }
            };
            await _appDbContext.Exercicios.AddRangeAsync(listaExercicios);
            await _appDbContext.SaveChangesAsync();

            var treino = new Treino
            {
                Id = 1,
                Nome="Full body",
                Data=new DateTime(),
                Exercicios= null      
            };

            var listaTreinoExercicio = new List<TreinoExercicio>
            {
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[0].Id, Exercicio= listaExercicios[0] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[1].Id, Exercicio= listaExercicios[1] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[2].Id, Exercicio= listaExercicios[2] },
            };

            treino.Exercicios = listaTreinoExercicio;

            await _appDbContext.TreinoExercicios.AddRangeAsync(listaTreinoExercicio);
            await _appDbContext.SaveChangesAsync();

            // Act
            var result = await _treinosController.GetAll();

            // Assert
            Assert.That((HttpStatusCode)((OkObjectResult)result).StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<List<Treino>>(((OkObjectResult) result).Value);
            Assert.AreEqual(treino.Exercicios.Count, ((List<Treino>)((OkObjectResult)result).Value)[0].Exercicios.Count);
        }

        [Test]
        public async Task TreinosController_Create_RetornaCreated_QuandoTreinoCriadoComSucess()
        {
            // Arrange
            var listaExercicios = new List<Exercicio>
            {
                new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 },
                new Exercicio { Id = 2, Nome = "Agachamento", Series = 4, Repeticoes = 12 },
                new Exercicio { Id = 3, Nome = "Rosca direta", Series = 3, Repeticoes = 10 }
            };
            await _appDbContext.Exercicios.AddRangeAsync(listaExercicios);
            await _appDbContext.SaveChangesAsync();

            var treino = new Treino
            {
                Id = 1,
                Nome = "Full body",
                Exercicios = null
            };

            var listaTreinoExercicio = new List<TreinoExercicio>
            {
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[0].Id, Exercicio= listaExercicios[0] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[1].Id, Exercicio= listaExercicios[1] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[2].Id, Exercicio= listaExercicios[2] },
            };

            treino.Exercicios = listaTreinoExercicio;

            // Act
            var result = await _treinosController.Create(treino);

            // Assert
            Assert.That((HttpStatusCode)((CreatedAtActionResult)result).StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            Assert.AreEqual("GetById", ((CreatedAtActionResult)result).ActionName);
            Assert.AreEqual(treino.Id, ((Treino)((CreatedAtActionResult)result).Value).Id);
        }

        [Test]
        public async Task TreinosController_GetById_RetornaNotFound_QuandoTreinoENulo()
        {
            // Act
            var result = await _treinosController.GetById(0);

            // Assert
            Assert.That((HttpStatusCode)((NotFoundResult)result).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TreinosController_GetById_RetornaExercicio_QuandoTreinoExiste()
        {
            // Arrange
            var listaExercicios = new List<Exercicio>
            {
                new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 },
                new Exercicio { Id = 2, Nome = "Agachamento", Series = 4, Repeticoes = 12 },
                new Exercicio { Id = 3, Nome = "Rosca direta", Series = 3, Repeticoes = 10 }
            };
            await _appDbContext.Exercicios.AddRangeAsync(listaExercicios);
            await _appDbContext.SaveChangesAsync();

            var treino = new Treino
            {
                Id = 1,
                Nome = "Full body",
                Exercicios = null
            };

            var listaTreinoExercicio = new List<TreinoExercicio>
            {
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[0].Id, Exercicio= listaExercicios[0] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[1].Id, Exercicio= listaExercicios[1] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[2].Id, Exercicio= listaExercicios[2] },
            };

            treino.Exercicios = listaTreinoExercicio;

            await _appDbContext.TreinoExercicios.AddRangeAsync(listaTreinoExercicio);
            await _appDbContext.SaveChangesAsync();

            // Act
            var result = await _treinosController.GetById(treino.Id);

            // Assert
            Assert.That((HttpStatusCode)((OkObjectResult)result).StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsInstanceOf<Treino>(((OkObjectResult)result).Value);
            Assert.AreEqual(treino.Id, ((Treino)((OkObjectResult)result).Value).Id);
        }

        [Test]
        public async Task TreinosController_Update_RetornaBadRequest_QuandoIdDiferenteDoModel()
        {
            // Arrange
            var listaExercicios = new List<Exercicio>
            {
                new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 },
                new Exercicio { Id = 2, Nome = "Agachamento", Series = 4, Repeticoes = 12 },
                new Exercicio { Id = 3, Nome = "Rosca direta", Series = 3, Repeticoes = 10 }
            };
            await _appDbContext.Exercicios.AddRangeAsync(listaExercicios);
            await _appDbContext.SaveChangesAsync();

            var treino = new Treino
            {
                Id = 1,
                Nome = "Full body",
                Exercicios = null
            };

            var listaTreinoExercicio = new List<TreinoExercicio>
            {
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[0].Id, Exercicio= listaExercicios[0] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[1].Id, Exercicio= listaExercicios[1] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[2].Id, Exercicio= listaExercicios[2] },
            };

            treino.Exercicios = listaTreinoExercicio;

            await _appDbContext.TreinoExercicios.AddRangeAsync(listaTreinoExercicio);
            await _appDbContext.SaveChangesAsync();

            // Act
            var result = await _treinosController.Update(2, treino);

            // Assert
            Assert.That((HttpStatusCode)((BadRequestResult)result).StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.IsInstanceOf<BadRequestResult>(result);
        }
        [Test]
        public async Task TreinosController_Update_RetornaNotFound_QuandoTreinoNaoExiste()
        {
            // Arrange
            var listaExercicios = new List<Exercicio>
            {
                new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 },
                new Exercicio { Id = 2, Nome = "Agachamento", Series = 4, Repeticoes = 12 },
                new Exercicio { Id = 3, Nome = "Rosca direta", Series = 3, Repeticoes = 10 }
            };

            var treino = new Treino
            {
                Id = 1,
                Nome = "Full body",
                Exercicios = null
            };

            var listaTreinoExercicio = new List<TreinoExercicio>
            {
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[0].Id, Exercicio= listaExercicios[0] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[1].Id, Exercicio= listaExercicios[1] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[2].Id, Exercicio= listaExercicios[2] },
            };

            treino.Exercicios = listaTreinoExercicio;

            // Act
            var result = await _treinosController.Update(1, treino);

            // Assert
            Assert.That((HttpStatusCode)((NotFoundResult)result).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task TreinosController_Update_RetornaNoContent_QuandoTreinoAtualizadoComSucesso()
        {
            // Arrange
            var listaExercicios = new List<Exercicio>
            {
                new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 },
                new Exercicio { Id = 2, Nome = "Agachamento", Series = 4, Repeticoes = 12 },
                new Exercicio { Id = 3, Nome = "Rosca direta", Series = 3, Repeticoes = 10 }
            };
            await _appDbContext.Exercicios.AddRangeAsync(listaExercicios);
            await _appDbContext.SaveChangesAsync();

            var treino = new Treino
            {
                Id = 1,
                Nome = "Full body",
                Exercicios = null
            };

            var listaTreinoExercicio = new List<TreinoExercicio>
            {
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[0].Id, Exercicio= listaExercicios[0] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[1].Id, Exercicio= listaExercicios[1] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[2].Id, Exercicio= listaExercicios[2] },
            };

            treino.Exercicios = listaTreinoExercicio;

            await _appDbContext.TreinoExercicios.AddRangeAsync(listaTreinoExercicio);
            await _appDbContext.SaveChangesAsync();

            treino.Nome = "All body";

            // Act
            var result = await _treinosController.Update(1, treino);
            var treinoAtualizado = await _appDbContext.Treinos.FindAsync(treino.Id);

            // Assert
            Assert.That((HttpStatusCode)((NoContentResult)result).StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.IsInstanceOf<NoContentResult>(result);
            Assert.AreEqual("All body", treinoAtualizado.Nome);
        }

        [Test]
        public async Task TreinosController_Delete_RetornaNotFound_QuandoTreinoNaoExiste()
        {
            // Act
            var result = await _treinosController.Delete(1);

            // Assert
            Assert.That((HttpStatusCode)((NotFoundResult)result).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task TreinosController_Delete_RetornaNoContent_QuandoTreinoExcluidoComSucesso()
        {
            // Arrange
            var listaExercicios = new List<Exercicio>
            {
                new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 },
                new Exercicio { Id = 2, Nome = "Agachamento", Series = 4, Repeticoes = 12 },
                new Exercicio { Id = 3, Nome = "Rosca direta", Series = 3, Repeticoes = 10 }
            };
            await _appDbContext.Exercicios.AddRangeAsync(listaExercicios);
            await _appDbContext.SaveChangesAsync();

            var treino = new Treino
            {
                Id = 1,
                Nome = "Full body",
                Exercicios = null
            };

            var listaTreinoExercicio = new List<TreinoExercicio>
            {
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[0].Id, Exercicio= listaExercicios[0] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[1].Id, Exercicio= listaExercicios[1] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[2].Id, Exercicio= listaExercicios[2] },
            };

            treino.Exercicios = listaTreinoExercicio;

            await _appDbContext.TreinoExercicios.AddRangeAsync(listaTreinoExercicio);
            await _appDbContext.SaveChangesAsync();

            // Act
            var result = await _treinosController.Delete(1);
            var treinoExcluido = await _appDbContext.Treinos.FindAsync(treino.Id);

            // Assert
            Assert.That((HttpStatusCode)((OkObjectResult)result).StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsNull(treinoExcluido);
        }

        

        [Test]
        public async Task TreinosController_DeleteExercicio_RetornaNoContent_QuandoExercicioExcluidoComSucesso()
        {
            // Arrange
            var listaExercicios = new List<Exercicio>
            {
                new Exercicio { Id = 1, Nome = "Supino", Series = 3, Repeticoes = 10 },
                new Exercicio { Id = 2, Nome = "Agachamento", Series = 4, Repeticoes = 12 },
                new Exercicio { Id = 3, Nome = "Rosca direta", Series = 3, Repeticoes = 10 }
            };
            await _appDbContext.Exercicios.AddRangeAsync(listaExercicios);
            await _appDbContext.SaveChangesAsync();

            var treino = new Treino
            {
                Id = 1,
                Nome = "Full body",
                Exercicios = null
            };

            var listaTreinoExercicio = new List<TreinoExercicio>
            {
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[0].Id, Exercicio= listaExercicios[0] },
                new TreinoExercicio { TreinoId= treino.Id, Treino= treino, ExercicioId=listaExercicios[1].Id, Exercicio= listaExercicios[1] }
            };

            treino.Exercicios = listaTreinoExercicio;

            await _appDbContext.TreinoExercicios.AddRangeAsync(listaTreinoExercicio);
            await _appDbContext.SaveChangesAsync();

            var treinoExercicio = new TreinoExercicio { TreinoId = treino.Id, Treino = treino, ExercicioId = listaExercicios[2].Id, Exercicio = listaExercicios[2] };

            // Act
            var result = await _treinosController.DeleteExercicio(1, 1);

            // Assert
            Assert.That((HttpStatusCode)((NoContentResult)result).StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.IsInstanceOf<NoContentResult>(result);
        }

    }
}
