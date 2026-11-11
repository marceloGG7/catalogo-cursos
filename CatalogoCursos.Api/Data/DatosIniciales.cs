using CatalogoCursos.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CatalogoCursos.Api.Data;

public static class DatosIniciales
{
    public static async Task CrearUsuariosAsync(
        IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var hasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<Usuario>>();

        var usuarios = new[]
        {
            new
            {
                Nombre = "Administrador",
                Correo = "admin@catalogo.test",
                Password = "AdminDemo2026!",
                Rol = "admin"
            },
            new
            {
                Nombre = "Usuario de prueba",
                Correo = "usuario@catalogo.test",
                Password = "UsuarioDemo2026!",
                Rol = "user"
            }
        };

        foreach (var datos in usuarios)
        {
            var existe = await context.Usuarios
                .AnyAsync(u => u.Correo == datos.Correo);

            if (existe)
            {
                continue;
            }

            var usuario = new Usuario
            {
                Nombre = datos.Nombre,
                Correo = datos.Correo,
                Rol = datos.Rol
            };

            usuario.PasswordHash = hasher.HashPassword(
                usuario, datos.Password);

            context.Usuarios.Add(usuario);
        }

        await context.SaveChangesAsync();
    }
}