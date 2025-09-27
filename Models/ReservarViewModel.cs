namespace PortalInmobiliario.Models
{
    public class ReservarViewModel
    {
        public int InmuebleId { get; set; }
        public string InmuebleTitulo { get; set; }
        public DateTime FechaExpiracion { get; set; } = DateTime.Now.AddHours(48);
    }
}