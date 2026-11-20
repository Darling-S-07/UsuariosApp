using System.Text.Json;

namespace UsuariosApp.Services;

/// <summary>Convierte excepciones técnicas en mensajes entendibles para el usuario.</summary>
public static class ErrorHelper
{
    public static string ObtenerMensaje(Exception ex) => ex switch
    {
        HttpRequestException { StatusCode: { } codigo }
            => $"El servidor respondió con un error ({(int)codigo} {codigo}).",

        HttpRequestException
            => "No se pudo conectar con el servidor. Revisa tu conexión a internet.",

        // HttpClient lanza TaskCanceledException cuando se agota el Timeout.
        OperationCanceledException
            => "La solicitud tardó demasiado. Inténtalo de nuevo.",

        JsonException
            => "La respuesta del servidor no tiene el formato esperado.",

        _ => $"Ocurrió un error inesperado: {ex.Message}"
    };
}
