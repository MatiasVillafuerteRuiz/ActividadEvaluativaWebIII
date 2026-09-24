using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestionClinicaVeterinaria.Models;

namespace GestionClinicaVeterinaria.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<ServicioVeterinario> ServiciosVeterinarios { get; set; }
        public DbSet<Cita> Citas { get; set; }
    }
}