using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalInmobiliario.Models
{
    public class Inmueble
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Display(Name = "Imagen")]
        public string? Imagen { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio")]
        [Display(Name = "Tipo de Inmueble")]
        public TipoInmueble Tipo { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [Display(Name = "Ciudad")]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El número de dormitorios es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "Los dormitorios no pueden ser negativos")]
        [Display(Name = "Dormitorios")]
        public int Dormitorios { get; set; }

        [Required(ErrorMessage = "El número de baños es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "Los baños no pueden ser negativos")]
        [Display(Name = "Baños")]
        public int Banos { get; set; }

        [Required(ErrorMessage = "Los metros cuadrados son obligatorios")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Los metros cuadrados deben ser mayores a 0")]
        [Display(Name = "Metros Cuadrados")]
        public double MetrosCuadrados { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Display(Name = "Precio")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Visita> Visitas { get; set; } = new List<Visita>();
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }

    public enum TipoInmueble
    {
        Departamento,
        Casa,
        Oficina,
        Local
    }
}