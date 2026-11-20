namespace UsuariosApp.Services;

/// <summary>
/// Permite que los ViewModels muestren mensajes al usuario
/// sin depender directamente de las páginas.
/// </summary>
public interface IDialogService
{
    Task MostrarAlertaAsync(string titulo, string mensaje, string aceptar = "Aceptar");
}

public class DialogService : IDialogService
{
    // En .NET 10, DisplayAlert se marcó como obsoleto a favor de DisplayAlertAsync.
    // El pragma evita la advertencia y el código compila igual en .NET 8, 9 y 10.
#pragma warning disable CS0618
    public Task MostrarAlertaAsync(string titulo, string mensaje, string aceptar = "Aceptar")
        => Shell.Current.DisplayAlert(titulo, mensaje, aceptar);
#pragma warning restore CS0618
}
