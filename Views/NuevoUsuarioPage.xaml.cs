using UsuariosApp.ViewModels;

namespace UsuariosApp.Views;

public partial class NuevoUsuarioPage : ContentPage
{
    public NuevoUsuarioPage(NuevoUsuarioViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
