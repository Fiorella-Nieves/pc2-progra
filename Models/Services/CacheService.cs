using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PortalInmobiliario.Models;
using PortalInmobiliario.Data;
using System.Text;

namespace PortalInmobiliario.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(1); // 60 segundos como solicitado

        public CacheService(IDistributedCache cache, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<List<Inmueble>> GetInmueblesCachedAsync(InmuebleFilterViewModel filtro, Func<Task<List<Inmueble>>> getFromDb)
        {
            var cacheKey = GenerateCacheKey(filtro);
            
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonConvert.DeserializeObject<List<Inmueble>>(cachedData);
            }

            var inmuebles = await getFromDb();
            
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            };
            
            await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(inmuebles), cacheOptions);
            
            return inmuebles;
        }

        public async Task InvalidateInmueblesCacheAsync()
        {
            // Invalidar todas las claves de caché de inmuebles
            // En una implementación real, usaríamos Redis patterns o mantener una lista de claves
            // Por simplicidad, invalidamos solo las claves principales
            var pattern = "PortalInmobiliario_*inmuebles*";
            // Nota: Redis no soporta pattern deletion directamente, necesitarías SCAN en producción
            // Para esta demo, simplemente no invalidamos todo el caché
        }

        public async Task GuardarFiltrosEnSesionAsync(InmuebleFilterViewModel filtro)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var filtroJson = JsonConvert.SerializeObject(filtro);
                session.SetString("UltimosFiltros", filtroJson);
            }
        }

        public async Task<InmuebleFilterViewModel?> ObtenerFiltrosDeSesionAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var filtroJson = session.GetString("UltimosFiltros");
                if (!string.IsNullOrEmpty(filtroJson))
                {
                    return JsonConvert.DeserializeObject<InmuebleFilterViewModel>(filtroJson);
                }
            }
            return null;
        }

        public async Task GuardarUltimoInmuebleVisitadoAsync(int inmuebleId, string titulo)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetInt32("UltimoInmuebleId", inmuebleId);
                session.SetString("UltimoInmuebleTitulo", titulo);
            }
        }

        public async Task<(int? inmuebleId, string? titulo)> ObtenerUltimoInmuebleVisitadoAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var id = session.GetInt32("UltimoInmuebleId");
                var titulo = session.GetString("UltimoInmuebleTitulo");
                return (id, titulo);
            }
            return (null, null);
        }

        private string GenerateCacheKey(InmuebleFilterViewModel filtro)
        {
            var keyParts = new List<string> { "inmuebles" };
            
            if (!string.IsNullOrEmpty(filtro.Ciudad))
                keyParts.Add($"ciudad_{filtro.Ciudad}");
            
            if (filtro.Tipo.HasValue)
                keyParts.Add($"tipo_{filtro.Tipo}");
            
            if (filtro.PrecioMin.HasValue)
                keyParts.Add($"pmin_{filtro.PrecioMin}");
            
            if (filtro.PrecioMax.HasValue)
                keyParts.Add($"pmax_{filtro.PrecioMax}");
            
            if (filtro.Dormitorios.HasValue)
                keyParts.Add($"dorm_{filtro.Dormitorios}");
            
            keyParts.Add($"page_{filtro.Pagina}");
            
            return string.Join("_", keyParts);
        }
    }
}