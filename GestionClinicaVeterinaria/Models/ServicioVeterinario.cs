using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionClinicaVeterinaria.Models
{
    public class ServicioVeterinario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del servicio es obligatorio")]
        [StringLength(100)]
        public string NombreServicio { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 10000.00, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        // Relación con Citas
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}