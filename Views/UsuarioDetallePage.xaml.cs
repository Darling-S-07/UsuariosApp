using UsuariosApp.ViewModels;

namespace UsuariosApp.Views;

public partial class UsuarioDetallePage : ContentPage
{
    public UsuarioDetallePage(UsuarioDetalleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
