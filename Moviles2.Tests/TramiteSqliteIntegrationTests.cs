using System.Threading.Tasks;
using SQLite;
using Xunit;
using Moviles2.Models;

namespace Moviles2.Tests
{
    public class TramiteSqliteIntegrationTests
    {
        [Fact]
        public async Task SQLiteInMemory_GuardarYLeerTramite_PersisteDatosCorrectamente()
        {
            // Arrange: Base SQLite real en memoria RAM
            var db = new SQLiteAsyncConnection(":memory:");
            await db.CreateTableAsync<TramiteVigilador>();

            var tramite = new TramiteVigilador
            {
                NombreSolicitante = "Sofía Martínez",
                TieneDNI = true,
                TieneBiometricos = true,
                TieneSecundario = true,
                TieneCurso = true,
                TieneReincidencia = true,
                EstaPresentado = true
            };
            tramite.AprobarHabilitacion();

            // Act
            await db.InsertAsync(tramite);
            var recuperado = await db.Table<TramiteVigilador>().FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(recuperado);
            Assert.Equal("Sofía Martínez", recuperado!.NombreSolicitante);
            Assert.True(recuperado.EstaHabilitado);
            Assert.Equal("Presentado - Habilitado", recuperado.EstadoGeneral);

            await db.CloseAsync();
        }
    }
}