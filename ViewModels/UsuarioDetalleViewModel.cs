using System.Windows.Input;
using UsuariosApp.Models;
using UsuariosApp.Services;

namespace UsuariosApp.ViewModels;

/// <summary>
/// Detalle de un usuario. Recibe el Id como parámetro de navegación
/// (IQueryAttributable) y consulta GET /users/{id}.
/// </summary>
public class UsuarioDetalleViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IApiService _api;
    private readonly DatabaseService _db;
    private readonly IDialogService _dialogo;

    private int _usuarioId;
    private Usuario? _usuarioActual;

    public UsuarioDetalleViewModel(IApiService api, DatabaseService db, IDialogService dialogo)
    {
        _api = api;
        _db = db;
        _dialogo = dialogo;

        ReintentarCommand = new Command(async () => await CargarAsync());
    }

    public int UsuarioId
    {
        get => _usuarioId;
        private set => SetProperty(ref _usuarioId, value);
    }

    public Usuario? UsuarioActual
    {
        get => _usuarioActual;
        private set
        {
            if (SetProperty(ref _usuarioActual, value))
            {
                OnPropertyChanged(nameof(TieneUsuario));
                OnPropertyChanged(nameof(MostrarReintentar));
                OnPropertyChanged(nameof(Inicial));
            }
        }
    }

    public bool TieneUsuario => UsuarioActual is not null;
    public bool MostrarReintentar => !IsBusy && UsuarioActual is null;

    /// <summary>Primera letra del nombre, para el avatar.</summary>
    public string Inicial =>
        UsuarioActual?.Name is { Length: > 0 } nombre ? nombre[..1].ToUpperInvariant() : "?";

    public ICommand ReintentarCommand { get; }

    protected override void OnIsBusyChanged() => OnPropertyChanged(nameof(MostrarReintentar));

    /// <summary>Shell llama a este método con los parámetros de la ruta (detalle?id=3).</summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var valor) && int.TryParse(valor?.ToString(), out var id))
        {
            UsuarioId = id;
            _ = CargarAsync();
        }
    }

    private async Task CargarAsync()
    {
        if (IsBusy || UsuarioId <= 0)
            return;

        Usuario? usuario = null;
        string? error = null;
        var desdeCache = false;

        IsBusy = true;
        try
        {
            usuario = await _api.GetUsuarioAsync(UsuarioId);
            await GuardarEnCacheAsync(usuario);
        }
        catch (Exception ex)
        {
            error = ErrorHelper.ObtenerMensaje(ex);
            usuario = await LeerCacheAsync(UsuarioId);
            desdeCache = usuario is not null;
        }
        finally
        {
            IsBusy = false;
        }

        UsuarioActual = usuario;

        if (error is not null)
        {
            var mensaje = desdeCache
                ? $"{error}\n\nSe muestran los datos guardados en el dispositivo."
                : error;
            await _dialogo.MostrarAlertaAsync(desdeCache ? "Sin conexión" : "Error", mensaje);
        }
    }

    private async Task GuardarEnCacheAsync(Usuario usuario)
    {
        try
        {
            await _db.GuardarUsuarioAsync(usuario);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"No se pudo guardar el caché: {ex.Message}");
        }
    }

    private async Task<Usuario?> LeerCacheAsync(int id)
    {
        try
        {
            return await _db.ObtenerUsuarioAsync(id);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"No se pudo leer el caché: {ex.Message}");
            return null;
        }
    }
}
