using System.IO;
using System.Threading.Tasks;
using SQLite;
using Moviles2.Models;

namespace Moviles2.Repositories
{
    public class TramiteRepositorySQLite
    {
        private SQLiteAsyncConnection _database;

        public TramiteRepositorySQLite()
        {
            
            var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "tramites.db3");

            
            _database = new SQLiteAsyncConnection(dbPath);

            
            _database.CreateTableAsync<TramiteVigilador>().Wait();
        }

        public async Task<TramiteVigilador> ObtenerTramiteAsync()
        {
            // Buscamos si ya hay un trámite guardado
            var tramite = await _database.Table<TramiteVigilador>().FirstOrDefaultAsync();

            // Si no hay nada, creamos uno vacío
            if (tramite == null)
            {
                tramite = new TramiteVigilador { NombreSolicitante = "" };
                await _database.InsertAsync(tramite);
            }

            return tramite;
        }

        public async Task GuardarTramiteAsync(TramiteVigilador tramite)
        {
            // Sobreescribe los datos viejos con los nuevos
            await _database.UpdateAsync(tramite);
        }
    }
}