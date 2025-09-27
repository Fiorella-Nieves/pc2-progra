using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalInmobiliario.Models;

namespace PortalInmobiliario.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Verificar si ya existe el rol Broker
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

                string[] roleNames = { "Broker" };

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Crear usuario broker por defecto
                var brokerEmail = "broker@inmobiliaria.com";
                var brokerUser = await userManager.FindByEmailAsync(brokerEmail);

                if (brokerUser == null)
                {
                    brokerUser = new IdentityUser
                    {
                        UserName = brokerEmail,
                        Email = brokerEmail,
                        EmailConfirmed = true
                    };

                    var createResult = await userManager.CreateAsync(brokerUser, "Broker123!");
                    
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(brokerUser, "Broker");
                    }
                }

                // Verificar si ya hay inmuebles en la base de datos
                if (!context.Inmuebles.Any())
                {
                    context.Inmuebles.AddRange(
                        new Inmueble
                        {
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
                        }
                    );

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}