using Microsoft.AspNetCore.Mvc;

namespace PortalInmobiliario.Areas.Broker
{
    public class BrokerAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Broker";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Broker_default",
                pattern: "Broker/{controller=Inmuebles}/{action=Index}/{id?}");
        }
    }
}