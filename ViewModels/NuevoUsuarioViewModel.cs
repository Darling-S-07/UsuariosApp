using System.Net.Mail;
using System.Windows.Input;
using UsuariosApp.Models;
using UsuariosApp.Services;

namespace UsuariosApp.ViewModels;

/// <summary>Formulario que envía un usuario nuevo con POST /users y muestra el Id devuelto.</summary>
public class NuevoUsuarioViewModel : BaseViewModel
{
    private readonly IApiService _api;
    private readonly IDialogService _dialogo;

    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _phone = string.Empty;
    private string _website = string.Empty;
    private Usuario? _usuarioCreado;

    public NuevoUsuarioViewModel(IApiService api, IDialogService dialogo)
    {
        _api = api;
        _dialogo = dialogo;

        EnviarCommand = new Command(async () => await EnviarAsync());
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    public string Website
    {
        get => _website;
        set => SetProperty(ref _website, value);
    }

    /// <summary>Respuesta de la API (incluye el Id asignado).</summary>
    public Usuario? UsuarioCreado
    {
        get => _usuarioCreado;
        private set
        {
            if (SetProperty(ref _usuarioCreado, value))
                OnPropertyChanged(nameof(MostrarResultado));
        }
    }

    public bool MostrarResultado => UsuarioCreado is not null;

    public ICommand EnviarCommand { get; }

    private async Task EnviarAsync()
    {
        if (IsBusy)
            return;

        var errorValidacion = Validar();
        if (errorValidacion is not null)
        {
            await _dialogo.MostrarAlertaAsync("Datos incompletos", errorValidacion);
            return;
        }

        var nuevo = new Usuario
        {
            Name = Name.Trim(),
            Email = Email.Trim(),
            Phone = Phone.Trim(),
            Website = Website.Trim()
        };

        Usuario? creado = null;
        string? error = null;

        IsBusy = true;
        try
        {
            creado = await _api.CrearUsuarioAsync(nuevo);
        }
        catch (Exception ex)
        {
            error = ErrorHelper.ObtenerMensaje(ex);
        }
        finally
        {
            IsBusy = false;
        }

        if (error is not null || creado is null)
        {
            await _dialogo.MostrarAlertaAsync("Error", error ?? "No se pudo crear el usuario.");
            return;
        }

        UsuarioCreado = creado;
        LimpiarFormulario();
        await _dialogo.MostrarAlertaAsync("Usuario creado",
            $"La API devolvió el Id {creado.Id} para \"{creado.Name}\".");
    }

    private string? Validar()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return "El nombre es obligatorio.";

        if (string.IsNullOrWhiteSpace(Email))
            return "El correo es obligatorio.";

        if (!EsCorreoValido(Email))
            return "El correo no tiene un formato válido (ejemplo: ana@correo.com).";

        return null;
    }

    private static bool EsCorreoValido(string correo)
    {
        var texto = correo.Trim();
        try
        {
            return new MailAddress(texto).Address == texto;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private void LimpiarFormulario()
    {
        Name = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Website = string.Empty;
    }
}
