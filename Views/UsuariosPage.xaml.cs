using UsuariosApp.ViewModels;

namespace UsuariosApp.Views;

public partial class UsuariosPage : ContentPage
{
    private readonly UsuariosViewModel _viewModel;

    public UsuariosPage(UsuariosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Carga la lista solo la primera vez que aparece la página.
        await _viewModel.InicializarAsync();
    }
}
