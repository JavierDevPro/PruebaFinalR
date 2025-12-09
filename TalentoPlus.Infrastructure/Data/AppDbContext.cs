using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    // DbSets
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // ========== CONFIGURACIONES DE USUARIO ==========
        modelBuilder.Entity<Usuario>(entity =>
        {
            // Índice único para Email
            entity.HasIndex(u => u.Email).IsUnique();
            
            // Relación con Role
            entity.HasOne(u => u.Role)
                  .WithMany(r => r.Usuarios)
                  .HasForeignKey(u => u.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            // Relación 1:1 con Empleado
            entity.HasOne(u => u.Empleado)
                  .WithOne(e => e.Usuario)
                  .HasForeignKey<Usuario>(u => u.EmpleadoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // ========== CONFIGURACIONES DE EMPLEADO ==========
        modelBuilder.Entity<Empleado>(entity =>
        {
            // Índices únicos
            entity.HasIndex(e => e.Documento).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            // Relación con Departamento
            entity.HasOne(e => e.Departamento)
                  .WithMany(d => d.Empleados)
                  .HasForeignKey(e => e.DepartamentoId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            // Relación 1:1 con Usuario
            entity.HasOne(e => e.Usuario)
                  .WithOne(u => u.Empleado)
                  .HasForeignKey<Empleado>(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // ========== CONFIGURACIONES DE ROLE ==========
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(r => r.Nombre).IsUnique();
        });
        
        // ========== CONFIGURACIONES DE DEPARTAMENTO ==========
        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasIndex(d => d.Nombre).IsUnique();
        });
        
        // ========== SEED DATA ==========
        
        // Seed de Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Nombre = "Admin", Descripcion = "Administrador del sistema" },
            new Role { Id = 2, Nombre = "Empleado", Descripcion = "Empleado de la empresa" }
        );
        
        // Seed de Departamentos (5 departamentos)
        modelBuilder.Entity<Departamento>().HasData(
            new Departamento { Id = 1, Nombre = "Recursos Humanos", Descripcion = "Gestión de personal" },
            new Departamento { Id = 2, Nombre = "Tecnología", Descripcion = "Desarrollo y soporte IT" },
            new Departamento { Id = 3, Nombre = "Operaciones", Descripcion = "Operaciones y logística" },
            new Departamento { Id = 4, Nombre = "Marketing", Descripcion = "Mercadeo y publicidad" },
            new Departamento { Id = 5, Nombre = "Logística", Descripcion = "Cadena de suministro" }
        );
        
        // Seed de Usuario Admin (sin empleado asociado)
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                Email = "admin@talento.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // Hasheado correctamente
                RoleId = 1, // Admin
                EmpleadoId = null, // Admin no tiene empleado asociado
                RefreshToken = null,
                RefreshTokenExpire = null
            }
        );
        
        // NOTA: Ya NO hay seed de Empleado Admin porque Admin es solo Usuario
        // Los empleados se importarán desde el Excel
    }
}