using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UsuariosApp.Models;

namespace UsuariosApp.Services;

/// <summary>
/// Único lugar de la app que habla con la API REST (HttpClient).
/// No atrapa excepciones: las deja subir para que los ViewModels
/// las manejen con try/catch y muestren un mensaje al usuario.
/// </summary>
public class ApiService : IApiService
{
    // La barra final es importante para que las rutas relativas ("users") se resuelvan bien.
    private const string BaseUrl = "https://jsonplaceholder.typicode.com/";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;

    public ApiService()
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(15)
        };
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("UsuariosApp/1.0");
    }

    public async Task<List<Usuario>> GetUsuariosAsync(CancellationToken ct = default)
    {
        using var response = await _http.GetAsync("users", ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<List<Usuario>>(json, JsonOptions) ?? new List<Usuario>();
    }

    public async Task<Usuario> GetUsuarioAsync(int id, CancellationToken ct = default)
    {
        using var response = await _http.GetAsync($"users/{id}", ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<Usuario>(json, JsonOptions)
               ?? throw new JsonException("La respuesta de la API llegó vacía.");
    }

    public async Task<Usuario> CrearUsuarioAsync(Usuario usuario, CancellationToken ct = default)
    {
        var jsonEnvio = JsonSerializer.Serialize(usuario, JsonOptions);
        using var contenido = new StringContent(jsonEnvio, Encoding.UTF8, "application/json");

        using var response = await _http.PostAsync("users", contenido, ct);
        response.EnsureSuccessStatusCode();

        var jsonRespuesta = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<Usuario>(jsonRespuesta, JsonOptions)
               ?? throw new JsonException("La respuesta de la API llegó vacía.");
    }
}
