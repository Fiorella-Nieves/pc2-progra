using PortalInmobiliario.Models;

namespace PortalInmobiliario.Services
{
    public interface ICacheService
    {
        Task<List<Inmueble>> GetInmueblesCachedAsync(InmuebleFilterViewModel filtro, Func<Task<List<Inmueble>>> getFromDb);
        Task InvalidateInmueblesCacheAsync();
        Task GuardarFiltrosEnSesionAsync(InmuebleFilterViewModel filtro);
        Task<InmuebleFilterViewModel?> ObtenerFiltrosDeSesionAsync();
        Task GuardarUltimoInmuebleVisitadoAsync(int inmuebleId, string titulo);
        Task<(int? inmuebleId, string? titulo)> ObtenerUltimoInmuebleVisitadoAsync();
    }
}