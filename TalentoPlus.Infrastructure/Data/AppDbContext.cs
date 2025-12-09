using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Índices únicos
        modelBuilder.Entity<Empleado>()
            .HasIndex(e => e.Documento)
            .IsUnique();
            
        modelBuilder.Entity<Empleado>()
            .HasIndex(e => e.Email)
            .IsUnique();
            
        // Relaciones
        modelBuilder.Entity<Empleado>()
            .HasOne(e => e.Departamento)
            .WithMany(d => d.Empleados)
            .HasForeignKey(e => e.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Seed de departamentos
        modelBuilder.Entity<Departamento>().HasData(
            new Departamento { Id = 1, Nombre = "Recursos Humanos", Descripcion = "Gestión de personal" },
            new Departamento { Id = 2, Nombre = "Tecnología", Descripcion = "Desarrollo y soporte IT" },
            new Departamento { Id = 3, Nombre = "Operaciones", Descripcion = "Operaciones y logística" },
            new Departamento { Id = 4, Nombre = "Marketing", Descripcion = "Mercadeo y publicidad" },
            new Departamento { Id = 5, Nombre = "Logística", Descripcion = "Cadena de suministro" }
        );
        
        // Seed de admin - FECHAS CON DateTimeKind.Utc
        modelBuilder.Entity<Empleado>().HasData(
            new Empleado
            {
                Id = 1,
                Documento = "12345678",
                Nombres = "Admin",
                Apellidos = "Sistema",
                Email = "admin@talento.com",
                PasswordHash = "$2a$11$4R3M8XyZ1A2B3C4D5E6F7G8H9I0J1K2L3M4N5O6P7Q8R9S0T1U2V3W4X5Y6Z7", // Admin123!
                Role = "Admin",
                DepartamentoId = 1,
                Cargo = "Administrador",
                Salario = 0,
                // FECHAS ESPECIFICANDO DateTimeKind.Utc
                FechaIngreso = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                FechaNacimiento = new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Estado = "Activo",
                NivelEducativo = "Profesional",
                Direccion = "Oficina Principal",
                Telefono = "3001234567",
                PerfilProfesional = "Administrador del sistema"
            }
        );
    }
}