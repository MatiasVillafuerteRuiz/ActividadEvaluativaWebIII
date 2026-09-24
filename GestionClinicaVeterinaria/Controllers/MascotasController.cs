using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionClinicaVeterinaria.Data;
using GestionClinicaVeterinaria.Models;

namespace GestionClinicaVeterinaria.Controllers
{
    [Authorize] // Solo usuarios autenticados
    public class MascotasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MascotasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Mascotas
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var mascotas = await _context.Mascotas
                .Where(m => m.UserId == userId)
                .ToListAsync();

            return View(mascotas);
        }

        // GET: Mascotas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mascotas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mascota mascota)
        {
            var userId = _userManager.GetUserId(User);
            mascota.UserId = userId!;

            // Remover validación del objeto Usuario para evitar falsos InvalidState
            ModelState.Remove("UserId");
            ModelState.Remove("Usuario");

            if (ModelState.IsValid)
            {
                _context.Add(mascota);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mascota);
        }
    }
}