using GestionClinicaVeterinaria.Data;
using GestionClinicaVeterinaria.Models.ViewModels;
using GestionClinicaVeterinaria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionClinicaVeterinaria.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ReportePdfService _reportePdfService;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public ReportesController(
            ApplicationDbContext context,
            ReportePdfService reportePdfService)
        {
            _context = context;
            _reportePdfService = reportePdfService;
        }


        // =========================================================
        // PÁGINA PRINCIPAL DE REPORTES
        // =========================================================

        public async Task<IActionResult> Index()
        {
            // Obtenemos únicamente los usuarios
            // que tienen el rol Cliente.

            var clientes = await (
                from user in _context.Users

                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId

                join role in _context.Roles
                    on userRole.RoleId equals role.Id

                where role.Name == "Cliente"

                orderby user.Nombre, user.Apellido

                select user

            ).ToListAsync();


            var modelo = new ReportesViewModel
            {
                Clientes = clientes
            };


            return View(modelo);
        }


        // =========================================================
        // REPORTE GENERAL DE CITAS
        // =========================================================

        public async Task<IActionResult> ReporteGeneral()
        {
            var citas = await _context.Citas

                // Mascota
                .Include(c => c.Mascota)

                    // Dueño de la mascota
                    .ThenInclude(m => m.Usuario)

                // Servicio veterinario
                .Include(c => c.ServicioVeterinario)

                // Ordenamos por fecha
                .OrderByDescending(c => c.FechaHora)

                .ToListAsync();


            // Generar PDF
            var pdf = _reportePdfService
                .GenerarReporteGeneral(citas);


            // Descargar archivo
            return File(
                pdf,
                "application/pdf",
                "ReporteGeneralCitas.pdf");
        }


        // =========================================================
        // REPORTE DE CITAS POR CLIENTE
        // =========================================================

        public async Task<IActionResult> ReportePorCliente(
            string clienteId)
        {
            // -----------------------------------------------------
            // Validar que se haya seleccionado un cliente
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(clienteId))
            {
                TempData["Error"] =
                    "Debe seleccionar un cliente.";

                return RedirectToAction(nameof(Index));
            }


            // -----------------------------------------------------
            // Buscar cliente
            // -----------------------------------------------------

            var cliente = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == clienteId);


            if (cliente == null)
            {
                return NotFound();
            }


            // -----------------------------------------------------
            // Buscar las citas de las mascotas de ese cliente
            // -----------------------------------------------------

            var citas = await _context.Citas

                .Include(c => c.Mascota)

                .Include(c => c.ServicioVeterinario)

                .Where(c =>
                    c.Mascota != null &&
                    c.Mascota.UserId == clienteId)

                .OrderByDescending(c => c.FechaHora)

                .ToListAsync();


            // -----------------------------------------------------
            // Generar PDF
            // -----------------------------------------------------

            var pdf = _reportePdfService
                .GenerarReportePorCliente(
                    cliente,
                    citas);


            // -----------------------------------------------------
            // Nombre seguro para el archivo
            // -----------------------------------------------------

            var nombre =
                string.IsNullOrWhiteSpace(cliente.Nombre)
                    ? "Cliente"
                    : cliente.Nombre;

            var apellido =
                string.IsNullOrWhiteSpace(cliente.Apellido)
                    ? ""
                    : cliente.Apellido;


            return File(
                pdf,
                "application/pdf",
                $"Citas_{nombre}_{apellido}.pdf");
        }


        // =========================================================
        // REPORTE DE SERVICIOS MÁS SOLICITADOS
        // =========================================================

        public async Task<IActionResult>
            ReporteServiciosMasSolicitados()
        {
            // Agrupamos la información usando la relación
            // ServicioVeterinario -> Citas.

            var servicios =
                await _context.ServiciosVeterinarios

                    .Select(servicio =>
                        new ServicioSolicitadoViewModel
                        {
                            NombreServicio =
                                servicio.NombreServicio,

                            Precio =
                                servicio.Precio,

                            CantidadCitas =
                                servicio.Citas.Count()
                        })

                    // Solamente servicios que hayan sido solicitados
                    .Where(servicio =>
                        servicio.CantidadCitas > 0)

                    // Más solicitados primero
                    .OrderByDescending(servicio =>
                        servicio.CantidadCitas)

                    // Si tienen la misma cantidad,
                    // ordenar alfabéticamente
                    .ThenBy(servicio =>
                        servicio.NombreServicio)

                    .ToListAsync();


            // -----------------------------------------------------
            // Generar PDF
            // -----------------------------------------------------

            var pdf = _reportePdfService
                .GenerarReporteServiciosMasSolicitados(
                    servicios);


            return File(
                pdf,
                "application/pdf",
                "ServiciosMasSolicitados.pdf");
        }
    }
}