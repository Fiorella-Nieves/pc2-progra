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
        }

        // GET: Inmuebles
        public async Task<IActionResult> Index(InmuebleFilterViewModel filtro)
        {
            // Validaciones server-side
            if (filtro.PrecioMin.HasValue && filtro.PrecioMax.HasValue && filtro.PrecioMin > filtro.PrecioMax)
            {
                ModelState.AddModelError("PrecioMax", "El precio máximo debe ser mayor o igual al precio mínimo");
            }

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

            return View(inmueble);
        }

        // POST: Inmuebles/Reservar/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar(int id)
        {
            var inmueble = await _context.Inmuebles
                .FirstOrDefaultAsync(i => i.Id == id && i.Activo);

            if (inmueble == null)
            {
                TempData["Error"] = "Inmueble no encontrado o no disponible.";
                return RedirectToAction("Index");
            }

            // Validar que no exista reserva activa
            var reservaActiva = await _context.Reservas
                .AnyAsync(r => r.InmuebleId == id && r.FechaExpiracion > DateTime.Now);

            if (reservaActiva)
            {
                TempData["Error"] = "Este inmueble ya tiene una reserva activa.";
                return RedirectToAction("Details", new { id });
            }

            var reserva = new Reserva
            {
                InmuebleId = id,
                UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                FechaCreacion = DateTime.Now,
                FechaExpiracion = DateTime.Now.AddHours(48) // 48 horas de reserva
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Inmueble reservado exitosamente. La reserva expira el {reserva.FechaExpiracion:dd/MM/yyyy a las HH:mm}.";
            return RedirectToAction("Details", new { id });
        }

        // Método privado para aplicar filtros y preparar el ViewModel
        private async Task<InmuebleListViewModel> AplicarFiltrosYVista(InmuebleFilterViewModel filtro)
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

            // Paginación
            var totalItems = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(totalItems / (double)ITEMS_POR_PAGINA);
            var paginaActual = Math.Max(1, Math.Min(filtro.Pagina, totalPaginas));

            var inmuebles = await query
                .OrderBy(i => i.Id)
                .Skip((paginaActual - 1) * ITEMS_POR_PAGINA)
                .Take(ITEMS_POR_PAGINA)
                .ToListAsync();

            return new InmuebleListViewModel
            {
                Inmuebles = inmuebles,
                Filtro = filtro,
                PaginaActual = paginaActual,
                TotalPaginas = totalPaginas,
                TotalItems = totalItems
            };
        }
    }

    // ViewModel para la lista de inmuebles
    public class InmuebleListViewModel
    {
        public List<Inmueble> Inmuebles { get; set; } = new List<Inmueble>();
        public InmuebleFilterViewModel Filtro { get; set; } = new InmuebleFilterViewModel();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalItems { get; set; } = 0;
    }
}