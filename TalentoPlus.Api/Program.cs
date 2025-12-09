using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddInfrastructure(builder.Configuration);

// Repositories
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
// AutoMapper
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(TalentoPlus.Application.Mapping.MappingProfile)));

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TalentoPlus API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed admin if not exists
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Ensure database is created
    dbContext.Database.EnsureCreated();
    
    // Seed admin if not exists
    if (!dbContext.Empleados.Any(e => e.Role == "Admin"))
    {
        var admin = new TalentoPlus.Domain.Entities.Empleado
        {
            Documento = "12345678",
            Nombres = "Admin",
            Apellidos = "Sistema",
            Email = "admin@talento.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin",
            DepartamentoId = 1,
            Cargo = "Administrador",
            Salario = 0,
            FechaIngreso = DateTime.UtcNow,
            FechaNacimiento = new DateTime(1980, 1, 1),
            Estado = "Activo",
            NivelEducativo = "Profesional",
            Direccion = "Oficina Principal",
            Telefono = "3001234567",
            PerfilProfesional = "Administrador del sistema"
        };
        
        dbContext.Empleados.Add(admin);
        dbContext.SaveChanges();
        Console.WriteLine("Admin user created: admin@talento.com / Admin123!");
    }
}

app.Run();