using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalInmobiliario.Models;
using PortalInmobiliario.Data;
using Microsoft.AspNetCore.Authorization;

namespace PortalInmobiliario.Areas.Broker.Controllers
{
    [Area("Broker")]
    [Authorize(Roles = "Broker")]
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Broker/Reservas
        public async Task<IActionResult> Index()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Inmueble)
                .Include(r => r.Usuario)
                .Where(r => r.FechaExpiracion > DateTime.Now)
                .OrderBy(r => r.FechaExpiracion)
                .ToListAsync();

            return View(reservas);
        }

        // POST: Broker/Reservas/Liberar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Liberar(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Inmueble)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            // En lugar de eliminar, establecer fecha de expiración en el pasado
            reserva.FechaExpiracion = DateTime.Now.AddMinutes(-1);
            _context.Update(reserva);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Reserva liberada para {reserva.Inmueble.Titulo}.";
            return RedirectToAction(nameof(Index));
        }
    }
}