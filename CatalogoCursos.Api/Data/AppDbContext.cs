using CatalogoCursos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoCursos.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>()
            .HasIndex(c => c.Nombre)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Correo)
            .IsUnique();

        modelBuilder.Entity<Curso>()
            .HasOne(c => c.Categoria)
            .WithMany()
            .HasForeignKey(c => c.CategoriaId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}