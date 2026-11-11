using CatalogoCursos.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogoCursos.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerCategorias()
    {
        var categorias = await _context.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .Select(c => new
            {
                c.Id,
                c.Nombre
            })
            .ToListAsync();

        return Ok(categorias);
    }
}