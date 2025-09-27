using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalInmobiliario.Models;
using PortalInmobiliario.Data;
using Microsoft.AspNetCore.Authorization;

namespace PortalInmobiliario.Areas.Broker.Controllers
{
    [Area("Broker")]
    [Authorize(Roles = "Broker")]
    public class VisitasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Broker/Visitas/Agenda
        public async Task<IActionResult> Agenda(DateTime? fecha)
        {
            var fechaFiltro = fecha ?? DateTime.Today;
            
            var visitas = await _context.Visitas
                .Include(v => v.Inmueble)
                .Include(v => v.Usuario)
                .Where(v => v.FechaInicio.Date == fechaFiltro.Date)
                .OrderBy(v => v.FechaInicio)
                .ToListAsync();

            ViewBag.FechaSeleccionada = fechaFiltro;
            ViewBag.FechaAnterior = fechaFiltro.AddDays(-1);
            ViewBag.FechaSiguiente = fechaFiltro.AddDays(1);

            return View(visitas);
        }

        // POST: Broker/Visitas/Confirmar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int id)
        {
            var visita = await _context.Visitas
                .Include(v => v.Inmueble)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visita == null)
            {
                return NotFound();
            }

            visita.Estado = EstadoVisita.Confirmada;
            _context.Update(visita);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Visita confirmada para {visita.Inmueble.Titulo}.";
            return RedirectToAction(nameof(Agenda), new { fecha = visita.FechaInicio.Date });
        }

        // POST: Broker/Visitas/Cancelar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var visita = await _context.Visitas
                .Include(v => v.Inmueble)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visita == null)
            {
                return NotFound();
            }

            visita.Estado = EstadoVisita.Cancelada;
            _context.Update(visita);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Visita cancelada para {visita.Inmueble.Titulo}.";
            return RedirectToAction(nameof(Agenda), new { fecha = visita.FechaInicio.Date });
        }
    }
}