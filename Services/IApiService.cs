using UsuariosApp.Models;

namespace UsuariosApp.Services;

public interface IApiService
{
    /// <summary>GET /users</summary>
    Task<List<Usuario>> GetUsuariosAsync(CancellationToken ct = default);

    /// <summary>GET /users/{id}</summary>
    Task<Usuario> GetUsuarioAsync(int id, CancellationToken ct = default);

    /// <summary>POST /users — devuelve el usuario con el Id asignado por la API.</summary>
    Task<Usuario> CrearUsuarioAsync(Usuario usuario, CancellationToken ct = default);
}
