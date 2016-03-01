using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using itcast.crm19.IServices;
using itcast.crm19.model;
using itcast.crm19.WebHelper.Attrs;
using itcast.crm19.WebHelper;
namespace itcast.crm19.Site.Areas.admin.Controllers
{
    [SkipCheckPermiss]
    public class GetFunctionController : BaseController
    {
        //
        // GET: /admin/GetFunction/

        public GetFunctionController(IsysPermissListServices permissSer)
        {
            base.permissSer = permissSer;
        }

        public ActionResult GenerateFunction(int id)
        {
            int mid=id;
            int uid = UserManager.LoginedUserInfo().uID;
            var functionList = permissSer.RunProc<sysFunction>("Usp_GetFunctionsByUid19 " + uid + "," + mid);
            List<object> resultList = new List<object>();
            if (functionList != null && functionList.Any())
            {
                foreach (sysFunction item in functionList)
                {
                    resultList.Add(new
                    {

                        text = item.fName,
                        click = item.fFunction,
                        icon = item.fPicname
                    });
                    resultList.Add(new { line = true });
                }
            }
            return Json(resultList,JsonRequestBehavior.AllowGet);
        }
    }
}
