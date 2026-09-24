using GestionClinicaVeterinaria.Data;
using GestionClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionClinicaVeterinaria.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ServiciosVeterinariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiciosVeterinariosController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // LISTAR SERVICIOS
        // GET: ServiciosVeterinarios
        // =========================================================

        public async Task<IActionResult> Index()
        {
            var servicios = await _context.ServiciosVeterinarios
                .OrderBy(s => s.NombreServicio)
                .ToListAsync();

            return View(servicios);
        }


        // =========================================================
        // DETALLES
        // GET: ServiciosVeterinarios/Details/5
        // =========================================================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.ServiciosVeterinarios
                .FirstOrDefaultAsync(s => s.Id == id);

            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }


        // =========================================================
        // CREAR - GET
        // =========================================================

        public IActionResult Create()
        {
            return View();
        }


        // =========================================================
        // CREAR - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("NombreServicio,Descripcion,Precio")]
            ServicioVeterinario servicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicio);
                await _context.SaveChangesAsync();

                TempData["Exito"] =
                    "El servicio veterinario fue creado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(servicio);
        }


        // =========================================================
        // EDITAR - GET
        // =========================================================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio =
                await _context.ServiciosVeterinarios.FindAsync(id);

            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }


        // =========================================================
        // EDITAR - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,NombreServicio,Descripcion,Precio")]
            ServicioVeterinario servicio)
        {
            if (id != servicio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicio);
                    await _context.SaveChangesAsync();

                    TempData["Exito"] =
                        "El servicio veterinario fue actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicioVeterinarioExists(servicio.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(servicio);
        }


        // =========================================================
        // ELIMINAR - GET
        // =========================================================

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.ServiciosVeterinarios
                .FirstOrDefaultAsync(s => s.Id == id);

            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }


        // =========================================================
        // ELIMINAR - POST
        // =========================================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicio =
                await _context.ServiciosVeterinarios.FindAsync(id);

            if (servicio != null)
            {
                _context.ServiciosVeterinarios.Remove(servicio);
                await _context.SaveChangesAsync();

                TempData["Exito"] =
                    "El servicio veterinario fue eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // VERIFICAR EXISTENCIA
        // =========================================================

        private bool ServicioVeterinarioExists(int id)
        {
            return _context.ServiciosVeterinarios
                .Any(s => s.Id == id);
        }
    }
}