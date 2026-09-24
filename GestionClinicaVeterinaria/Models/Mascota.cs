using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionClinicaVeterinaria.Models
{
    public class Mascota
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la mascota es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especie es obligatoria")]
        [StringLength(30)]
        public string Especie { get; set; } = string.Empty; // Ej. Perro, Gato

        [StringLength(30)]
        public string? Raza { get; set; }

        [Range(0, 30, ErrorMessage = "La edad debe ser un valor válido")]
        public int Edad { get; set; }

        // Foreign Key del usuario (dueño)
        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? Usuario { get; set; }

        // Relación con Citas
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}