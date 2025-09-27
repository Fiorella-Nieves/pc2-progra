using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalInmobiliario.Models;
using PortalInmobiliario.Data;
using PortalInmobiliario.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PortalInmobiliario.Areas.Broker.Controllers
{
    [Area("Broker")]
    [Authorize(Roles = "Broker")]
    public class InmueblesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public InmueblesController(ApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        // GET: Broker/Inmuebles
        public async Task<IActionResult> Index()
        {
            var inmuebles = await _context.Inmuebles
                .OrderBy(i => i.Activo ? 0 : 1)
                .ThenBy(i => i.Id)
                .ToListAsync();

            return View(inmuebles);
        }

        // GET: Broker/Inmuebles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inmueble = await _context.Inmuebles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        // GET: Broker/Inmuebles/Create
        public IActionResult Create()
        {
            ViewBag.TiposInmueble = new SelectList(Enum.GetValues(typeof(TipoInmueble))
                .Cast<TipoInmueble>()
                .Select(t => new { Value = t, Text = t.ToString() }), 
                "Value", "Text");

            return View();
        }

        // POST: Broker/Inmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Titulo,Imagen,Tipo,Ciudad,Direccion,Dormitorios,Banos,MetrosCuadrados,Precio,Activo")] Inmueble inmueble)
        {
            // Validar código único
            if (await _context.Inmuebles.AnyAsync(i => i.Codigo == inmueble.Codigo))
            {
                ModelState.AddModelError("Codigo", "Ya existe un inmueble con este código.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(inmueble);
                await _context.SaveChangesAsync();
                
                // Invalidar caché
                await _cacheService.InvalidateInmueblesCacheAsync();
                
                TempData["Success"] = "Inmueble creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TiposInmueble = new SelectList(Enum.GetValues(typeof(TipoInmueble))
                .Cast<TipoInmueble>()
                .Select(t => new { Value = t, Text = t.ToString() }), 
                "Value", "Text", inmueble.Tipo);

            return View(inmueble);
        }

        // GET: Broker/Inmuebles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null)
            {
                return NotFound();
            }

            ViewBag.TiposInmueble = new SelectList(Enum.GetValues(typeof(TipoInmueble))
                .Cast<TipoInmueble>()
                .Select(t => new { Value = t, Text = t.ToString() }), 
                "Value", "Text", inmueble.Tipo);

            return View(inmueble);
        }

        // POST: Broker/Inmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Titulo,Imagen,Tipo,Ciudad,Direccion,Dormitorios,Banos,MetrosCuadrados,Precio,Activo")] Inmueble inmueble)
        {
            if (id != inmueble.Id)
            {
                return NotFound();
            }

            // Validar código único excluyendo el actual
            if (await _context.Inmuebles.AnyAsync(i => i.Codigo == inmueble.Codigo && i.Id != id))
            {
                ModelState.AddModelError("Codigo", "Ya existe un inmueble con este código.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inmueble);
                    await _context.SaveChangesAsync();
                    
                    // Invalidar caché
                    await _cacheService.InvalidateInmueblesCacheAsync();
                    
                    TempData["Success"] = "Inmueble actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InmuebleExists(inmueble.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TiposInmueble = new SelectList(Enum.GetValues(typeof(TipoInmueble))
                .Cast<TipoInmueble>()
                .Select(t => new { Value = t, Text = t.ToString() }), 
                "Value", "Text", inmueble.Tipo);

            return View(inmueble);
        }

        // POST: Broker/Inmuebles/ToggleActivo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null)
            {
                return NotFound();
            }

            inmueble.Activo = !inmueble.Activo;
            _context.Update(inmueble);
            await _context.SaveChangesAsync();
            
            // Invalidar caché
            await _cacheService.InvalidateInmueblesCacheAsync();
            
            TempData["Success"] = $"Inmueble {(inmueble.Activo ? "activado" : "desactivado")} correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool InmuebleExists(int id)
        {
            return _context.Inmuebles.Any(e => e.Id == id);
        }
    }
}