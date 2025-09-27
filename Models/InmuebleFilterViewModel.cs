using System.ComponentModel.DataAnnotations;

namespace PortalInmobiliario.Models
{
    public class InmuebleFilterViewModel
    {
        [Display(Name = "Ciudad")]
        public string? Ciudad { get; set; }

        [Display(Name = "Tipo de Inmueble")]
        public TipoInmueble? Tipo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo no puede ser negativo")]
        [Display(Name = "Precio Mínimo")]
        public decimal? PrecioMin { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio máximo no puede ser negativo")]
        [Display(Name = "Precio Máximo")]
        public decimal? PrecioMax { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Los dormitorios no pueden ser negativos")]
        [Display(Name = "Mín. Dormitorios")]
        public int? Dormitorios { get; set; }

        [Display(Name = "Página")]
        public int Pagina { get; set; } = 1;

        [Display(Name = "Items por Página")]
        public int ItemsPorPagina { get; set; } = 6;
    }
}