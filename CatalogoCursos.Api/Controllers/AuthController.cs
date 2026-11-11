using System.Security.Claims;
using CatalogoCursos.Api.Data;
using CatalogoCursos.Api.DTOs;
using CatalogoCursos.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogoCursos.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Usuario> _hasher;

    public AuthController(
        AppDbContext context,
        IPasswordHasher<Usuario> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto datos)
    {
        var correo = datos.Correo.Trim();

        var usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.Correo == correo);

        if (usuario is null)
        {
            return Unauthorized(new
            {
                mensaje = "Correo o contraseña incorrectos."
            });
        }

        var resultado = _hasher.VerifyHashedPassword(
            usuario,
            usuario.PasswordHash,
            datos.Password);

        if (resultado == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                mensaje = "Correo o contraseña incorrectos."
            });
        }

        if (resultado ==
            PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.PasswordHash = _hasher.HashPassword(
                usuario, datos.Password);

            await _context.SaveChangesAsync();
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var identidad = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidad),
            new AuthenticationProperties
            {
                IsPersistent = false
            });

        return Ok(new
        {
            usuario.Id,
            usuario.Nombre,
            usuario.Correo,
            usuario.Rol
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult ObtenerSesion()
    {
        return Ok(new
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Nombre = User.Identity?.Name,
            Correo = User.FindFirstValue(ClaimTypes.Email),
            Rol = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return NoContent();
    }
}