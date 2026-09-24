namespace GestionClinicaVeterinaria.Models.ViewModels
{
    public class ServicioSolicitadoViewModel
    {
        public string NombreServicio { get; set; } = string.Empty;

        public int CantidadCitas { get; set; }

        public decimal Precio { get; set; }
    }
}