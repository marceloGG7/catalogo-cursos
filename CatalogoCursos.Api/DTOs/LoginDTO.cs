using System.ComponentModel.DataAnnotations;

namespace CatalogoCursos.Api.DTOs;

public class LoginDto
{
    [Required, EmailAddress, MaxLength(150)]
    public string Correo { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Password { get; set; } = string.Empty;
}