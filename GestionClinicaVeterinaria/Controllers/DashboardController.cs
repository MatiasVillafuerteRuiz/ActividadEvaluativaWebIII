using GestionClinicaVeterinaria.Data;
using GestionClinicaVeterinaria.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionClinicaVeterinaria.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            // =====================================================
            // 1. DATOS GENERALES
            // =====================================================

            var modelo = new DashboardViewModel
            {
                TotalServicios =
                    await _context.ServiciosVeterinarios.CountAsync(),

                TotalMascotas =
                    await _context.Mascotas.CountAsync(),

                TotalClientes =
                    await (
                        from user in _context.Users
                        join userRole in _context.UserRoles
                            on user.Id equals userRole.UserId
                        join role in _context.Roles
                            on userRole.RoleId equals role.Id
                        where role.Name == "Cliente"
                        select user
                    ).CountAsync(),

                TotalCitas =
                    await _context.Citas.CountAsync(),

                CitasPendientes =
                    await _context.Citas
                        .CountAsync(c => c.Estado == "Pendiente"),

                CitasAtendidas =
                    await _context.Citas
                        .CountAsync(c => c.Estado == "Atendida"),

                CitasCanceladas =
                    await _context.Citas
                        .CountAsync(c => c.Estado == "Cancelada")
            };


            // =====================================================
            // 2. CONSULTAR CITAS AGRUPADAS POR MES
            // =====================================================

            var citasAgrupadas = await _context.Citas

                .GroupBy(c => new
                {
                    c.FechaHora.Year,
                    c.FechaHora.Month
                })

                .Select(grupo => new
                {
                    Anio = grupo.Key.Year,
                    Mes = grupo.Key.Month,
                    Cantidad = grupo.Count()
                })

                .OrderBy(x => x.Anio)
                .ThenBy(x => x.Mes)

                .ToListAsync();


            // =====================================================
            // 3. NOMBRES DE LOS MESES
            // =====================================================

            string[] nombresMeses =
            {
                "",
                "Enero",
                "Febrero",
                "Marzo",
                "Abril",
                "Mayo",
                "Junio",
                "Julio",
                "Agosto",
                "Septiembre",
                "Octubre",
                "Noviembre",
                "Diciembre"
            };


            // =====================================================
            // 4. PREPARAR DATOS PARA CHART.JS
            // =====================================================

            foreach (var item in citasAgrupadas)
            {
                modelo.Meses.Add(
                    $"{nombresMeses[item.Mes]} {item.Anio}");

                modelo.CitasPorMes.Add(
                    item.Cantidad);
            }


            return View(modelo);
        }
    }
}