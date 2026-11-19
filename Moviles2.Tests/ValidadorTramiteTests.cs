using Xunit;
using Moviles2.Models;
using Moviles2.Validators;

namespace Moviles2.Tests
{
    public class ValidadorTramiteTests
    {
        [Theory]
        [InlineData("Juan Perez", true)]
        [InlineData("Ana", true)]
        [InlineData("Al", false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData(null, false)]
        public void EsNombreValido_VariosEscenarios_RetornaResultadoEsperado(string? nombre, bool esperado)
        {
            // Act
            bool resultado = ValidadorTramite.EsNombreValido(nombre);

            // Assert
            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void PuedePresentarse_ConTodosLosRequisitos_RetornaTrue()
        {
            // Arrange
            var tramite = new TramiteVigilador
            {
                NombreSolicitante = "Carlos Gómez",
                TieneDNI = true,
                TieneBiometricos = true,
                TieneSecundario = true,
                TieneCurso = true,
                TieneReincidencia = true
            };

            // Act
            bool resultado = ValidadorTramite.PuedePresentarse(tramite);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void PuedePresentarse_ConRequisitosFaltantes_RetornaFalse()
        {
            // Arrange
            var tramite = new TramiteVigilador
            {
                NombreSolicitante = "Carlos Gómez",
                TieneDNI = true,
                TieneBiometricos = false,
                TieneSecundario = true,
                TieneCurso = true,
                TieneReincidencia = true
            };

            // Act
            bool resultado = ValidadorTramite.PuedePresentarse(tramite);

            // Assert
            Assert.False(resultado);
        }
    }
}