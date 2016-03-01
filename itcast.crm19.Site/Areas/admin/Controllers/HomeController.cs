using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using itcast.crm19.WebHelper.Attrs;
namespace itcast.crm19.Site.Areas.admin.Controllers
{
    using model;
    using IServices;
    using itcast.crm19.WebHelper;
    [SkipCheckPermiss]
    public class HomeController : Controller
    {
        IsysPermissListServices perSer;
        public HomeController(IsysPermissListServices perSer)
        {
            this.perSer = perSer;
        }
        //
        // GET: /admin/Home/

        public ActionResult Index()
        {        

            //1.0 获取当前登录用户的权限菜单
            int userId = UserManager.LoginedUserInfo().uID;
            var pList = perSer.RunProc<Usp_GetPermissMenu19_Result>("Usp_GetPermissMenu19 " + userId);

            ViewBag.plist = pList;

            return View();
        }

    }
}
