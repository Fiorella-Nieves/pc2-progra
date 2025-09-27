using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalInmobiliario.Models;
using PortalInmobiliario.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PortalInmobiliario.Controllers
{
    [Authorize]
    public class VisitasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Visitas/AgendarVisita/5
        public async Task<IActionResult> AgendarVisita(int inmuebleId)
        {
            var inmueble = await _context.Inmuebles
                .FirstOrDefaultAsync(i => i.Id == inmuebleId && i.Activo);

            if (inmueble == null)
            {
                TempData["Error"] = "Inmueble no encontrado o no disponible.";
                return RedirectToAction("Index", "Inmuebles");
            }

            var model = new AgendarVisitaViewModel
            {
                InmuebleId = inmueble.Id,
                InmuebleTitulo = inmueble.Titulo,
                FechaInicio = DateTime.Today.AddDays(1).AddHours(10), // Mañana a las 10 AM
                FechaFin = DateTime.Today.AddDays(1).AddHours(11)     // Mañana a las 11 AM
            };

            return View(model);
        }

        // POST: Visitas/AgendarVisita
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgendarVisita(AgendarVisitaViewModel model)
        {
            var inmueble = await _context.Inmuebles
                .FirstOrDefaultAsync(i => i.Id == model.InmuebleId && i.Activo);

            if (inmueble == null)
            {
                TempData["Error"] = "Inmueble no encontrado o no disponible.";
                return RedirectToAction("Index", "Inmuebles");
            }

            // Validaciones personalizadas
            if (model.FechaInicio >= model.FechaFin)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            // Validar horario laboral (8:00 - 19:00)
            if (model.FechaInicio.TimeOfDay < TimeSpan.FromHours(8) ||
                model.FechaFin.TimeOfDay > TimeSpan.FromHours(19))
            {
                ModelState.AddModelError("FechaInicio", "Las visitas solo pueden agendarse en horario laboral (8:00 - 19:00).");
            }

            // Validar que la fecha no sea en el pasado
            if (model.FechaInicio < DateTime.Now)
            {
                ModelState.AddModelError("FechaInicio", "No se pueden agendar visitas en fechas pasadas.");
            }

            // Validar solapamiento de visitas
            var visitaSolapada = await _context.Visitas
                .AnyAsync(v => v.InmuebleId == model.InmuebleId &&
                              v.Estado != EstadoVisita.Cancelada &&
                              ((model.FechaInicio >= v.FechaInicio && model.FechaInicio < v.FechaFin) ||
                               (model.FechaFin > v.FechaInicio && model.FechaFin <= v.FechaFin) ||
                               (model.FechaInicio <= v.FechaInicio && model.FechaFin >= v.FechaFin)));

            if (visitaSolapada)
            {
                ModelState.AddModelError("FechaInicio", "Ya existe una visita agendada para este inmueble en el horario seleccionado.");
            }

            if (!ModelState.IsValid)
            {
                model.InmuebleTitulo = inmueble.Titulo;
                return View(model);
            }

            var visita = new Visita
            {
                InmuebleId = model.InmuebleId,
                UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                FechaInicio = model.FechaInicio,
                FechaFin = model.FechaFin,
                Notas = model.Notas,
                Estado = EstadoVisita.Solicitada
            };

            _context.Visitas.Add(visita);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Visita agendada exitosamente para el {model.FechaInicio:dd/MM/yyyy} de {model.FechaInicio:HH:mm} a {model.FechaFin:HH:mm}.";
            return RedirectToAction("Details", "Inmuebles", new { id = model.InmuebleId });
        }
    }
}