using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalInmobiliario.Models
{
    public class Visita
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El inmueble es obligatorio")]
        [Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public string UsuarioId { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Estado")]
        public EstadoVisita Estado { get; set; } = EstadoVisita.Solicitada;

        [Display(Name = "Notas")]
        [StringLength(500, ErrorMessage = "Las notas no pueden exceder los 500 caracteres")]
        public string? Notas { get; set; }

        // Navigation properties
        [ForeignKey("InmuebleId")]
        public virtual Inmueble Inmueble { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser Usuario { get; set; }
    }

    public enum EstadoVisita
    {
        Solicitada,
        Confirmada,
        Cancelada
    }
}