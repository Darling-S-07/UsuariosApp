using SQLite;
using UsuariosApp.Models;

namespace UsuariosApp.Services;

/// <summary>
/// Caché local en SQLite (sqlite-net-pcl). Guarda los usuarios descargados
/// para poder mostrarlos si la API no está disponible (sin internet).
/// </summary>
public class DatabaseService
{
    private const string NombreArchivo = "usuarios.db3";

    private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

    private readonly SemaphoreSlim _candado = new(1, 1);
    private SQLiteAsyncConnection? _db;

    private async Task<SQLiteAsyncConnection> GetConexionAsync()
    {
        if (_db is not null)
            return _db;

        await _candado.WaitAsync();
        try
        {
            if (_db is null)
            {
                var ruta = Path.Combine(FileSystem.AppDataDirectory, NombreArchivo);
                var conexion = new SQLiteAsyncConnection(ruta, Flags);
                await conexion.CreateTableAsync<Usuario>();
                _db = conexion;
            }
            return _db;
        }
        finally
        {
            _candado.Release();
        }
    }

    public async Task<List<Usuario>> ObtenerUsuariosAsync()
    {
        var db = await GetConexionAsync();
        return await db.Table<Usuario>().OrderBy(u => u.Id).ToListAsync();
    }

    public async Task<Usuario?> ObtenerUsuarioAsync(int id)
    {
        var db = await GetConexionAsync();
        return await db.Table<Usuario>().Where(u => u.Id == id).FirstOrDefaultAsync();
    }

    /// <summary>Reemplaza todo el contenido de la tabla por la lista recibida.</summary>
    public async Task GuardarUsuariosAsync(IEnumerable<Usuario> usuarios)
    {
        var db = await GetConexionAsync();
        var lista = usuarios.ToList();
        await db.RunInTransactionAsync(conexion =>
        {
            conexion.DeleteAll<Usuario>();
            conexion.InsertAll(lista);
        });
    }

    public async Task GuardarUsuarioAsync(Usuario usuario)
    {
        var db = await GetConexionAsync();
        await db.InsertOrReplaceAsync(usuario);
    }
}
