using GestionClinicaVeterinaria.Models;

namespace GestionClinicaVeterinaria.Models.ViewModels
{
    public class ReportesViewModel
    {
        public List<ApplicationUser> Clientes { get; set; }
            = new List<ApplicationUser>();
    }
}