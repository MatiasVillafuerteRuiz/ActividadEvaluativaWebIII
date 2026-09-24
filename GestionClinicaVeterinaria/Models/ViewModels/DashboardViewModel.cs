namespace GestionClinicaVeterinaria.Models.ViewModels
{
    public class DashboardViewModel
    {
        // ==========================================
        // TOTALES GENERALES
        // ==========================================

        public int TotalServicios { get; set; }

        public int TotalMascotas { get; set; }

        public int TotalClientes { get; set; }

        public int TotalCitas { get; set; }


        // ==========================================
        // ESTADOS DE CITAS
        // ==========================================

        public int CitasPendientes { get; set; }

        public int CitasAtendidas { get; set; }

        public int CitasCanceladas { get; set; }


        // ==========================================
        // GRÁFICO
        // ==========================================

        public List<string> Meses { get; set; }
            = new List<string>();

        public List<int> CitasPorMes { get; set; }
            = new List<int>();
    }
}