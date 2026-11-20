using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UsuariosApp.ViewModels;

/// <summary>Base de todos los ViewModels: INotifyPropertyChanged + indicador de carga.</summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isBusy;

    /// <summary>true mientras hay una llamada en curso (controla el ActivityIndicator).</summary>
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(IsNotBusy));
                OnIsBusyChanged();
            }
        }
    }

    public bool IsNotBusy => !IsBusy;

    /// <summary>Los ViewModels hijos lo sobrescriben para notificar propiedades calculadas.</summary>
    protected virtual void OnIsBusyChanged() { }

    protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? nombre = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor))
            return false;

        campo = valor;
        OnPropertyChanged(nombre);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string? nombre = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
}
