using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalInmobiliario.Models;

namespace PortalInmobiliario.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Inmueble> Inmuebles { get; set; }
        public DbSet<Visita> Visitas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Inmueble
            modelBuilder.Entity<Inmueble>()
                .HasIndex(i => i.Codigo)
                .IsUnique();

            modelBuilder.Entity<Inmueble>()
                .Property(i => i.Precio)
                .HasColumnType("decimal(18,2)");

            // Configuración de Visita - Restricción de no solapamiento
            modelBuilder.Entity<Visita>()
                .HasCheckConstraint("CK_Visita_Fechas", "[FechaInicio] < [FechaFin]");

            // Configuración de Reserva
            modelBuilder.Entity<Reserva>()
                .HasCheckConstraint("CK_Reserva_Fechas", "[FechaCreacion] < [FechaExpiracion]");

            // Seed data
            modelBuilder.Entity<Inmueble>().HasData(
                new Inmueble
                {
                    Id = 1,
                    Codigo = "DEP-001",
                    Titulo = "Departamento Moderno en Centro",
                    Imagen = "/img/departamento1.jpg",
                    Tipo = TipoInmueble.Departamento,
                    Ciudad = "Lima",
                    Direccion = "Av. Arequipa 123",
                    Dormitorios = 2,
                    Banos = 2,
                    MetrosCuadrados = 85.5,
                    Precio = 150000m,
                    Activo = true
                },
                new Inmueble
                {
                    Id = 2,
                    Codigo = "CASA-001",
                    Titulo = "Casa Familiar en Surco",
                    Imagen = "/img/casa1.jpg",
                    Tipo = TipoInmueble.Casa,
                    Ciudad = "Lima",
                    Direccion = "Calle Los Pinos 456",
                    Dormitorios = 4,
                    Banos = 3,
                    MetrosCuadrados = 180.0,
                    Precio = 450000m,
                    Activo = true
                },
                new Inmueble
                {
                    Id = 3,
                    Codigo = "OFI-001",
                    Titulo = "Oficina en Torre Empresarial",
                    Imagen = "/img/oficina1.jpg",
                    Tipo = TipoInmueble.Oficina,
                    Ciudad = "San Isidro",
                    Direccion = "Av. Javier Prado 789",
                    Dormitorios = 0,
                    Banos = 2,
                    MetrosCuadrados = 60.0,
                    Precio = 120000m,
                    Activo = true
                },
                new Inmueble
                {
                    Id = 4,
                    Codigo = "LOC-001",
                    Titulo = "Local Comercial en Mall",
                    Imagen = "/img/local1.jpg",
                    Tipo = TipoInmueble.Local,
                    Ciudad = "Miraflores",
                    Direccion = "Centro Comercial Larcomar",
                    Dormitorios = 0,
                    Banos = 1,
                    MetrosCuadrados = 45.0,
                    Precio = 200000m,
                    Activo = false
                }
            );
        }
    }
}