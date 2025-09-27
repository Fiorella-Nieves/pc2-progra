using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalInmobiliario.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El inmueble es obligatorio")]
        [Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public string UsuarioId { get; set; }

        [Required(ErrorMessage = "La fecha de expiración es obligatoria")]
        [Display(Name = "Fecha de Expiración")]
        public DateTime FechaExpiracion { get; set; }

        [Required(ErrorMessage = "La fecha de creación es obligatoria")]
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("InmuebleId")]
        public virtual Inmueble Inmueble { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser Usuario { get; set; }
    }
}