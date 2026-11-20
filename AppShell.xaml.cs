using UsuariosApp.Views;

namespace UsuariosApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Rutas de las páginas a las que se navega con GoToAsync.
        Routing.RegisterRoute(Rutas.Detalle, typeof(UsuarioDetallePage));
        Routing.RegisterRoute(Rutas.NuevoUsuario, typeof(NuevoUsuarioPage));
    }
}
