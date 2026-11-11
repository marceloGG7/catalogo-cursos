using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogoCursos.Api.Models;

[Table("Usuarios")]
public class Usuario
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Correo { get; set; } = string.Empty;

    [JsonIgnore]
    [Required, MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, MaxLength(10)]
    public string Rol { get; set; } = "user";
}