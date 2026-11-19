using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Moviles2.Interfaces;
using Moviles2.Models;
using Moviles2.Services;

namespace Moviles2.Tests
{
    // Fake Repository en memoria (sin tocar SQLite real)
    public class TramiteRepositoryFake : ITramiteRepository
    {
        private TramiteVigilador? _guardado;

        public Task GuardarTramiteAsync(TramiteVigilador tramite)
        {
            _guardado = tramite;
            return Task.CompletedTask;
        }

        public Task<TramiteVigilador> ObtenerTramiteAsync()
        {
            return Task.FromResult(_guardado!);
        }
    }

    public class TramiteServiceTests
    {
        [Fact]
        public async Task GuardarProgresoAsync_NombreInvalido_LanzaArgumentException()
        {
            // Arrange
            var repoFake = new TramiteRepositoryFake();
            var service = new TramiteService(repoFake);
            var tramite = new TramiteVigilador { NombreSolicitante = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.GuardarProgresoAsync(tramite));
        }

        [Fact]
        public async Task GuardarProgresoAsync_NombreValido_GuardaEnFakeRepository()
        {
            // Arrange
            var repoFake = new TramiteRepositoryFake();
            var service = new TramiteService(repoFake);
            var tramite = new TramiteVigilador { NombreSolicitante = "Juan Pérez" };

            // Act
            bool resultado = await service.GuardarProgresoAsync(tramite);
            var recuperado = await service.ObtenerTramiteGuardadoAsync();

            // Assert
            Assert.True(resultado);
            Assert.NotNull(recuperado);
            Assert.Equal("Juan Pérez", recuperado!.NombreSolicitante);
        }

        [Fact]
        public async Task PresentarTramiteAsync_RequisitosIncompletos_RetornaFalseYNoGuarda()
        {
            // Arrange (usando Moq)
            var mockRepo = new Mock<ITramiteRepository>();
            var service = new TramiteService(mockRepo.Object);
            var tramite = new TramiteVigilador
            {
                NombreSolicitante = "Lucía Fernández",
                TieneDNI = true,
                TieneBiometricos = false
            };

            // Act
            bool resultado = await service.PresentarTramiteAsync(tramite);

            // Assert
            Assert.False(resultado);
            mockRepo.Verify(r => r.GuardarTramiteAsync(It.IsAny<TramiteVigilador>()), Times.Never);
        }

        [Fact]
        public async Task PresentarTramiteAsync_RequisitosCompletos_ApruebaYGuardaUnaVez()
        {
            // Arrange (usando Moq)
            var mockRepo = new Mock<ITramiteRepository>();
            var service = new TramiteService(mockRepo.Object);
            var tramite = new TramiteVigilador
            {
                NombreSolicitante = "Martín López",
                TieneDNI = true,
                TieneBiometricos = true,
                TieneSecundario = true,
                TieneCurso = true,
                TieneReincidencia = true
            };

            // Act
            bool resultado = await service.PresentarTramiteAsync(tramite);

            // Assert
            Assert.True(resultado);
            Assert.True(tramite.EstaPresentado);
            Assert.True(tramite.EstaHabilitado);
            Assert.NotNull(tramite.FechaVencimiento);
            mockRepo.Verify(r => r.GuardarTramiteAsync(tramite), Times.Once);
        }
    }
}