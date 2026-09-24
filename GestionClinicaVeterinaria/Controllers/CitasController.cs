using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionClinicaVeterinaria.Data;
using GestionClinicaVeterinaria.Models;

namespace GestionClinicaVeterinaria.Controllers
{
    [Authorize]
    public class CitasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CitasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Citas/MisCitas
        public async Task<IActionResult> MisCitas()
        {
            var userId = _userManager.GetUserId(User);

            var citas = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.ServicioVeterinario)
                .Where(c => c.Mascota!.UserId == userId)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync();

            return View(citas);
        }

        // GET: Citas/Solicitar
        public async Task<IActionResult> Solicitar()
        {
            var userId = _userManager.GetUserId(User);

            var misMascotas = await _context.Mascotas
                .Where(m => m.UserId == userId)
                .ToListAsync();

            ViewBag.MascotaId = new SelectList(misMascotas, "Id", "Nombre");
            ViewBag.ServicioVeterinarioId = new SelectList(await _context.ServiciosVeterinarios.ToListAsync(), "Id", "NombreServicio");

            return View();
        }

        // POST: Citas/Solicitar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(Cita cita)
        {
            var userId = _userManager.GetUserId(User);

            // Validación 1: Fecha futura
            if (cita.FechaHora <= DateTime.Now)
            {
                ModelState.AddModelError("FechaHora", "La fecha y hora de la cita debe ser posterior al momento actual.");
            }

            // Validación 2: Verificar pertenencia de la mascota al usuario autenticado
            var mascotaValida = await _context.Mascotas
                .AnyAsync(m => m.Id == cita.MascotaId && m.UserId == userId);

            if (!mascotaValida)
            {
                ModelState.AddModelError("MascotaId", "La mascota seleccionada no te pertenece o no es válida.");
            }

            ModelState.Remove("Mascota");
            ModelState.Remove("ServicioVeterinario");

            if (ModelState.IsValid)
            {
                cita.Estado = "Pendiente";
                _context.Add(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MisCitas));
            }

            // Recargar SelectLists si falla la validación
            var misMascotas = await _context.Mascotas.Where(m => m.UserId == userId).ToListAsync();
            ViewBag.MascotaId = new SelectList(misMascotas, "Id", "Nombre", cita.MascotaId);
            ViewBag.ServicioVeterinarioId = new SelectList(await _context.ServiciosVeterinarios.ToListAsync(), "Id", "NombreServicio", cita.ServicioVeterinarioId);

            return View(cita);
        }
    }
}