using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using itcast.crm19.IServices;
using itcast.crm19.model;
using itcast.crm19.WebHelper.Attrs;
namespace itcast.crm19.WebHelper.Filters
{
    public class CheckPermissAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Type type = typeof(SkipCheckPermissAttribute);
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(type, false)
                ||
                filterContext.ActionDescriptor.IsDefined(type, false))
            {
                return;
            }

            //获取区域 控制器 action名称
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string areaName = string.Empty;
            //判断当前 action是否为index
            if (actionName.Equals("index", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (filterContext.RouteData.DataTokens.ContainsKey("area"))
            {
                areaName = filterContext.RouteData.DataTokens["area"].ToString();
            }

            //获取当前用户的id

            int uid = UserManager.LoginedUserInfo().uID;
            //调用存储过程
            IsysPermissListServices PermissSer = Common.AutofacManager.GetInstance<IsysPermissListServices>();
            string procName = string.Format("Usp_GetFunctionData19 {0},'{1}','{2}','{3}'", uid, areaName, controllerName, actionName);

            var list = PermissSer.RunProc<sysPermissList>(procName);
            //返回数据则表示有权限 否则没有权限
            if (list == null || list.Any() == false)
            {
                NoPermiss(filterContext);
            }
        }
        private void NoPermiss(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                //返回jsonReuslt
                JsonResult json = new JsonResult();
                json.Data = new { status = 3, msg = "您没有权限访问此操作" };
                json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = json;
            }
            else
            {
                //返回ViewReuslt -> nopermiss.cshtml
                ViewResult view = new ViewResult();
                view.ViewName = "/Views/Shared/nopermiss.cshtml";
                filterContext.Result = view;
            }
        }

    }
}
