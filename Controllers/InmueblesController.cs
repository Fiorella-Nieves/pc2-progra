using Microsoft.AspNetCore.Mvc;
using PortalInmobiliario.Models;
using PortalInmobiliario.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PortalInmobiliario.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int ITEMS_POR_PAGINA = 6;

        public InmueblesController(ApplicationDbContext context)
        {
            _context = context;
            _cacheService = cacheService;
        }

        // GET: Inmuebles
        public async Task<IActionResult> Index(InmuebleFilterViewModel filtro)
        {
            // Validaciones server-side
            if (filtro.PrecioMin.HasValue && filtro.PrecioMax.HasValue && filtro.PrecioMin > filtro.PrecioMax)
            {
                ModelState.AddModelError("PrecioMax", "El precio máximo debe ser mayor o igual al precio mínimo");
            }

            await _cacheService.GuardarFiltrosEnSesionAsync(filtro);

            if (!ModelState.IsValid)
            {
                // Si hay errores, mantener los filtros pero mostrar errores
                return View(await AplicarFiltrosYVista(filtro));
            }

            return View(await AplicarFiltrosYVista(filtro));
        }

        // GET: Inmuebles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inmueble = await _context.Inmuebles
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);

            if (inmueble == null)
            {
                return NotFound();
            }

            await _cacheService.GuardarUltimoInmuebleVisitadoAsync(inmueble.Id, inmueble.Titulo);

            return View(inmueble);
        }
 private async Task<InmuebleListViewModel> AplicarFiltrosYVista(InmuebleFilterViewModel filtro)
        {
            // Usar caché para obtener inmuebles
            var inmuebles = await _cacheService.GetInmueblesCachedAsync(filtro, async () =>
            {
                var query = _context.Inmuebles.Where(i => i.Activo);

                // Aplicar filtros
                if (!string.IsNullOrEmpty(filtro.Ciudad))
                {
                    query = query.Where(i => i.Ciudad.Contains(filtro.Ciudad));
                }

                if (filtro.Tipo.HasValue)
                {
                    query = query.Where(i => i.Tipo == filtro.Tipo.Value);
                }

                if (filtro.PrecioMin.HasValue)
                {
                    query = query.Where(i => i.Precio >= filtro.PrecioMin.Value);
                }

                if (filtro.PrecioMax.HasValue)
                {
                    query = query.Where(i => i.Precio <= filtro.PrecioMax.Value);
                }

                if (filtro.Dormitorios.HasValue)
                {
                    query = query.Where(i => i.Dormitorios >= filtro.Dormitorios.Value);
                }

                return await query
                    .OrderBy(i => i.Id)
                    .ToListAsync();
            });

            // Paginación (fuera del caché para mayor flexibilidad)
            var totalItems = inmuebles.Count;
            var totalPaginas = (int)Math.Ceiling(totalItems / (double)ITEMS_POR_PAGINA);
            var paginaActual = Math.Max(1, Math.Min(filtro.Pagina, totalPaginas));

            var inmueblesPaginados = inmuebles
                .Skip((paginaActual - 1) * ITEMS_POR_PAGINA)
                .Take(ITEMS_POR_PAGINA)
                .ToList();

            return new InmuebleListViewModel
            {
                Inmuebles = inmueblesPaginados,
                Filtro = filtro,
                PaginaActual = paginaActual,
                TotalPaginas = totalPaginas,
                TotalItems = totalItems
            };
        }

        // Método para invalidar caché cuando se modifiquen inmuebles
        private async Task InvalidarCacheInmuebles()
        {
            await _cacheService.InvalidateInmueblesCacheAsync();
        }
    }
}