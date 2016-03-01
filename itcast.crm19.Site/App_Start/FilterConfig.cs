using itcast.crm19.WebHelper.Filters;
using System.Web;
using System.Web.Mvc;

namespace itcast.crm19.Site
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CheckLoginAttribute());
            filters.Add(new CheckPermissAttribute());
            filters.Add(new ErrorAttribute());

            
        }
    }
}