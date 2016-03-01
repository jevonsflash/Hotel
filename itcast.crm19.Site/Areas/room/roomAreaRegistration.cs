
using System.Web.Mvc;

namespace itcast.crm19.Site.Areas.room
{
    public class roomAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "room";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "room_default",
                "room/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
