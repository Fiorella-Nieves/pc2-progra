using Microsoft.EntityFrameworkCore;
using PortalInmobiliario.Data;
using PortalInmobiliario.Services;
using Microsoft.AspNetCore.Identity;
using PortalInmobiliario.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    // Usar variables de entorno de Render
    var redisConnectionString = builder.Configuration["Redis:ConnectionString"] 
                              ?? builder.Configuration.GetConnectionString("Redis");
    
    if (!string.IsNullOrEmpty(redisConnectionString))
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "PortalInmobiliario_";
        });
    }
    else
    {
        // Fallback a memoria distribuida si Redis no está disponible
        builder.Services.AddDistributedMemoryCache();
    }
}
else
{
    // Desarrollo: usar Redis local o memoria
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
    if (!string.IsNullOrEmpty(redisConnectionString) && redisConnectionString != "localhost:6379")
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "PortalInmobiliario_";
        });
    }
    else
    {
        builder.Services.AddDistributedMemoryCache();
    }
}

// Resto de la configuración...
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
});

// Conexión a la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configurar Redis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "PortalInmobiliario_";
});

// Configurar sesiones con Redis
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Registrar servicio de caché personalizado
builder.Services.AddScoped<ICacheService, CacheService>();

var app = builder.Build();

// Configuración de Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Servicios para MVC y Razor
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Añadir después de AddControllersWithViews()
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
        options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Aplicar migraciones automáticamente en producción
        if (app.Environment.IsProduction())
        {
            context.Database.Migrate();
        }
        
        // Ejecutar seed data
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error occurred during migration or seeding.");
    }
}

app.Run();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error occurred seeding the DB.");
    }
}

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseSession();
app.UseRouting();

if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Headers de seguridad
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();