using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionClinicaVeterinaria.Models
{
    public class Cita
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una fecha y hora")]
        [DataType(DataType.DateTime)]
        public DateTime FechaHora { get; set; }

        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmada, Cancelada

        [StringLength(250)]
        public string? Observaciones { get; set; }

        // Relación con Mascota
        [Required(ErrorMessage = "Debe seleccionar una mascota")]
        public int MascotaId { get; set; }

        [ForeignKey("MascotaId")]
        public Mascota? Mascota { get; set; }

        // Relación con ServicioVeterinario
        [Required(ErrorMessage = "Debe seleccionar un servicio")]
        public int ServicioVeterinarioId { get; set; }

        [ForeignKey("ServicioVeterinarioId")]
        public ServicioVeterinario? ServicioVeterinario { get; set; }
    }
}