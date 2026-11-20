using System.Collections.ObjectModel;
using System.Windows.Input;
using UsuariosApp.Models;
using UsuariosApp.Services;

namespace UsuariosApp.ViewModels;

/// <summary>Lista de usuarios: GET /users, RefreshView, selección y navegación al detalle.</summary>
public class UsuariosViewModel : BaseViewModel
{
    private readonly IApiService _api;
    private readonly DatabaseService _db;
    private readonly IDialogService _dialogo;

    private bool _isRefreshing;
    private bool _cargando;
    private bool _inicializado;
    private Usuario? _usuarioSeleccionado;

    public UsuariosViewModel(IApiService api, DatabaseService db, IDialogService dialogo)
    {
        _api = api;
        _db = db;
        _dialogo = dialogo;

        CargarCommand = new Command(async () => await CargarAsync(esRefresh: false));
        RefreshCommand = new Command(async () => await CargarAsync(esRefresh: true));
        VerDetalleCommand = new Command(async () => await VerDetalleAsync());
        NuevoUsuarioCommand = new Command(async () => await Shell.Current.GoToAsync(Rutas.NuevoUsuario));
    }

    public ObservableCollection<Usuario> Usuarios { get; } = new();

    /// <summary>Lo controla el RefreshView (deslizar hacia abajo).</summary>
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            if (SetProperty(ref _isRefreshing, value))
                OnPropertyChanged(nameof(MostrarVacio));
        }
    }

    /// <summary>true cuando no hay datos, no se está cargando y ya se intentó cargar al menos una vez.</summary>
    public bool MostrarVacio => _inicializado && !IsBusy && !IsRefreshing && Usuarios.Count == 0;

    public Usuario? UsuarioSeleccionado
    {
        get => _usuarioSeleccionado;
        set => SetProperty(ref _usuarioSeleccionado, value);
    }

    public ICommand CargarCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand VerDetalleCommand { get; }
    public ICommand NuevoUsuarioCommand { get; }

    protected override void OnIsBusyChanged() => OnPropertyChanged(nameof(MostrarVacio));

    /// <summary>Se llama desde OnAppearing; solo carga la primera vez.</summary>
    public async Task InicializarAsync()
    {
        if (_inicializado)
            return;

        _inicializado = true;
        await CargarAsync(esRefresh: false);
    }

    private async Task CargarAsync(bool esRefresh)
    {
        // Ojo: RefreshView pone IsRefreshing = true él solo antes de ejecutar el comando,
        // por eso la protección contra llamadas repetidas usa una bandera propia.
        if (_cargando)
        {
            if (esRefresh)
                IsRefreshing = false;
            return;
        }

        _cargando = true;
        _inicializado = true;
        IsBusy = !esRefresh;          // ActivityIndicator solo en la carga normal
        IsRefreshing = esRefresh;     // en el pull-to-refresh se ve el indicador del RefreshView

        List<Usuario>? lista = null;
        string? error = null;
        var desdeCache = false;

        try
        {
            lista = await _api.GetUsuariosAsync();
            await GuardarEnCacheAsync(lista);
        }
        catch (Exception ex)
        {
            error = ErrorHelper.ObtenerMensaje(ex);

            // Plan B: mostrar lo último que se guardó en SQLite.
            lista = await LeerCacheAsync();
            desdeCache = lista.Count > 0;
        }
        finally
        {
            _cargando = false;
            IsBusy = false;
            IsRefreshing = false;
        }

        if (lista is not null && (error is null || desdeCache))
            ReemplazarUsuarios(lista);

        // El mensaje se muestra al final para que el indicador de carga ya esté apagado.
        if (error is not null)
        {
            if (desdeCache)
                await _dialogo.MostrarAlertaAsync("Sin conexión",
                    $"{error}\n\nSe muestran los datos guardados en el dispositivo.");
            else
                await _dialogo.MostrarAlertaAsync("Error", error);
        }
    }

    private void ReemplazarUsuarios(IEnumerable<Usuario> nuevos)
    {
        Usuarios.Clear();
        foreach (var usuario in nuevos)
            Usuarios.Add(usuario);

        OnPropertyChanged(nameof(MostrarVacio));
    }

    private async Task VerDetalleAsync()
    {
        var usuario = UsuarioSeleccionado;
        if (usuario is null)
            return;

        // Se limpia la selección para poder tocar el mismo usuario otra vez al volver.
        UsuarioSeleccionado = null;

        // El Id viaja como parámetro en la ruta: detalle?id=3
        await Shell.Current.GoToAsync($"{Rutas.Detalle}?id={usuario.Id}");
    }

    private async Task GuardarEnCacheAsync(List<Usuario> lista)
    {
        try
        {
            await _db.GuardarUsuariosAsync(lista);
        }
        catch (Exception ex)
        {
            // Un fallo del caché no debe tumbar una respuesta correcta de la API.
            System.Diagnostics.Debug.WriteLine($"No se pudo guardar el caché: {ex.Message}");
        }
    }

    private async Task<List<Usuario>> LeerCacheAsync()
    {
        try
        {
            return await _db.ObtenerUsuariosAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"No se pudo leer el caché: {ex.Message}");
            return new List<Usuario>();
        }
    }
}
