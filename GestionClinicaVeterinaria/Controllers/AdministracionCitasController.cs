using GestionClinicaVeterinaria.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionClinicaVeterinaria.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministracionCitasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdministracionCitasController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // LISTAR TODAS LAS CITAS
        // =========================================================

        public async Task<IActionResult> Index()
        {
            var citas = await _context.Citas

                .Include(c => c.Mascota)
                    .ThenInclude(m => m.Usuario)

                .Include(c => c.ServicioVeterinario)

                .OrderByDescending(c => c.FechaHora)

                .ToListAsync();

            return View(citas);
        }


        // =========================================================
        // DETALLES DE UNA CITA
        // =========================================================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas

                .Include(c => c.Mascota)
                    .ThenInclude(m => m.Usuario)

                .Include(c => c.ServicioVeterinario)

                .FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }


        // =========================================================
        // CAMBIAR ESTADO
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            int id,
            string estado)
        {
            // -----------------------------------------------------
            // 1. Estados permitidos
            // -----------------------------------------------------

            string[] estadosPermitidos =
            {
                "Pendiente",
                "Atendida",
                "Cancelada"
            };


            // -----------------------------------------------------
            // 2. Validar el estado recibido
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(estado) ||
                !estadosPermitidos.Contains(estado))
            {
                TempData["Error"] =
                    "El estado seleccionado no es válido.";

                return RedirectToAction(nameof(Index));
            }


            // -----------------------------------------------------
            // 3. Buscar la cita
            // -----------------------------------------------------

            var cita = await _context.Citas
                .FindAsync(id);

            if (cita == null)
            {
                return NotFound();
            }


            // -----------------------------------------------------
            // 4. Actualizar estado
            // -----------------------------------------------------

            cita.Estado = estado;


            // -----------------------------------------------------
            // 5. Guardar
            // -----------------------------------------------------

            await _context.SaveChangesAsync();


            TempData["Exito"] =
                $"La cita fue actualizada a \"{estado}\".";


            return RedirectToAction(nameof(Index));
        }
    }
}