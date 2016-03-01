using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using itcast.crm19.WebHelper.Attrs;
namespace itcast.crm19.Site.Controllers
{
    using itcast.crm19.IServices;
    using itcast.crm19.IRepository;
    [SkipCheckPermiss]
    //注释
    public class HomeController : Controller
    {
        IsysFunctionServices funSer;
        IsysKeyValueServices keySer;

        public HomeController(IsysFunctionServices funSer, IsysKeyValueServices keySer)
        {
            this.funSer = funSer;
            this.keySer = keySer;
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return Redirect("/admin/Login/Login");
            //return new EmptyResult();
            //查询sysFunction表的fid=0数据 
            //这种形式的写法使UI层紧紧依赖于业务逻辑和仓储层
            //IsysFunctionRepository dal = new sysFunctionRepository();
            //IsysFunctionServices bll = new sysFunctionServices(dal);
            var model = funSer.QueryWhere(c => c.fID == 0).FirstOrDefault();
            model.fName += "1";
            //funSer.SaveChanges();

            var model1 = keySer.QueryWhere(c => c.KID == 2).FirstOrDefault();
            model1.KName += "1";

            //keySer.SaveChanges();
            funSer.SaveChanges();

            ViewBag.model1 = model1.KName;

            return View(model);
        }

    }
}
