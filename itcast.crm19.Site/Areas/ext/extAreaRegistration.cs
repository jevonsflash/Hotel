using System.Web.Mvc;

namespace itcast.crm19.Site.Areas.ext
{
    public class extAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ext";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ext_default",
                "ext/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
