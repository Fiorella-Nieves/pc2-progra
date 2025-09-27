using System.ComponentModel.DataAnnotations;

namespace PortalInmobiliario.Models
{
    public class AgendarVisitaViewModel
    {
        public int InmuebleId { get; set; }
        public string InmuebleTitulo { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Fecha y Hora de Inicio")]
        [DataType(DataType.DateTime)]
        public DateTime FechaInicio { get; set; } = DateTime.Today.AddHours(10);

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [Display(Name = "Fecha y Hora de Fin")]
        [DataType(DataType.DateTime)]
        public DateTime FechaFin { get; set; } = DateTime.Today.AddHours(11);

        [Display(Name = "Notas adicionales")]
        [StringLength(500, ErrorMessage = "Las notas no pueden exceder los 500 caracteres")]
        public string? Notas { get; set; }
    }
}