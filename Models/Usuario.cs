using System.Text.Json.Serialization;
using SQLite;

namespace UsuariosApp.Models;

/// <summary>
/// Modelo de usuario. Sirve a la vez para:
///  - deserializar el JSON de https://jsonplaceholder.typicode.com/users (System.Text.Json)
///  - guardarse en la tabla local de SQLite (sqlite-net-pcl)
/// La API devuelve más campos (address, company...), pero System.Text.Json
/// ignora los que no existen en la clase.
/// </summary>
[Table("Usuarios")]
public class Usuario
{
    // En el POST no se envía el Id (lo asigna el servidor), por eso se omite si vale 0.
    [PrimaryKey]
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("website")]
    public string Website { get; set; } = string.Empty;
}
