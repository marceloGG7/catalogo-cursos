using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogoCursos.Api.Models;

[Table("Cursos")]
public class Curso
{
    public int Id { get; set; }

    public int CategoriaId { get; set; }

    [Required, MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Descripcion { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImagenUrl { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }

    public Categoria Categoria { get; set; } = null!;
}