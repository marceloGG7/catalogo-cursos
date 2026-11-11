using CatalogoCursos.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatalogoCursos.Api.DTOs;
using CatalogoCursos.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace CatalogoCursos.Api.Controllers;

[ApiController]
[Route("api/cursos")]
public class CursosController : ControllerBase
{
    private readonly AppDbContext _context;

    public CursosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerCursos(
        [FromQuery] int? categoriaId)
    {
        if (categoriaId.HasValue && categoriaId.Value <= 0)
        {
            return BadRequest(new{mensaje = "El identificador de categoría debe ser positivo."});
        }

        var consulta = _context.Cursos.AsNoTracking();

        if (categoriaId.HasValue)
        {
            consulta = consulta.Where(c => c.CategoriaId == categoriaId.Value);
        }

        var cursos = await consulta
            .OrderBy(c => c.Id)
            .Select(c => new
            {
                c.Id,
                c.Titulo,
                c.Descripcion,
                c.ImagenUrl,
                c.Precio,
                c.CategoriaId,
                CategoriaNombre = c.Categoria.Nombre
            })
            .ToListAsync();
        return Ok(cursos);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerCurso(int id)
    {
        var curso = await _context.Cursos
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Titulo,
                c.Descripcion,
                c.ImagenUrl,
                c.Precio,
                c.CategoriaId,
                CategoriaNombre = c.Categoria.Nombre
            })
            .FirstOrDefaultAsync();

        if (curso is null)
        {
            return NotFound(new { mensaje = "El curso no existe." });
        }

        return Ok(curso);
    }
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> CrearCurso(
        [FromBody] CursoGuardarDto datos)
    {
        var categoriaExiste = await _context.Categorias
            .AnyAsync(c => c.Id == datos.CategoriaId);

        if (!categoriaExiste)
        {
            return BadRequest(new
            {
                mensaje = "La categoría indicada no existe."
            });
        }

        var curso = new Curso
        {
            CategoriaId = datos.CategoriaId,
            Titulo = datos.Titulo.Trim(),
            Descripcion = datos.Descripcion.Trim(),
            ImagenUrl = datos.ImagenUrl,
            Precio = datos.Precio
        };

        _context.Cursos.Add(curso);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(ObtenerCurso),
            new { id = curso.Id },
            new
            {
                curso.Id,
                curso.CategoriaId,
                curso.Titulo,
                curso.Descripcion,
                curso.ImagenUrl,
                curso.Precio
            });
    }
    [Authorize(Roles = "admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarCurso(
        int id,
        [FromBody] CursoGuardarDto datos)
    {
        var curso = await _context.Cursos.FindAsync(id);

        if (curso is null)
        {
            return NotFound(new { mensaje = "El curso no existe." });
        }

        var categoriaExiste = await _context.Categorias
            .AnyAsync(c => c.Id == datos.CategoriaId);

        if (!categoriaExiste)
        {
            return BadRequest(new
            {
                mensaje = "La categoría indicada no existe."
            });
        }

        curso.CategoriaId = datos.CategoriaId;
        curso.Titulo = datos.Titulo.Trim();
        curso.Descripcion = datos.Descripcion.Trim();
        curso.ImagenUrl = datos.ImagenUrl;
        curso.Precio = datos.Precio;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarCurso(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);

        if (curso is null)
        {
            return NotFound(new { mensaje = "El curso no existe." });
        }

        _context.Cursos.Remove(curso);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}