using System.ComponentModel.DataAnnotations;

namespace CatalogoCursos.Api.DTOs;

public class CursoGuardarDto
{
    [Range(1, int.MaxValue)]
    public int CategoriaId { get; set; }

    [Required, StringLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string Descripcion { get; set; } = string.Empty;

    [StringLength(500)]
    [Url]
    public string? ImagenUrl { get; set; }

    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal Precio { get; set; }
}