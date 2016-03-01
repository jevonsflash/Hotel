using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.WebHelper.Filters
{
    using itcast.crm19.Common;
    using System.Web.Mvc;
    using Attrs;
    using IServices;
    using System.Web.WebPages;

    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //0.0 判断如果贴了SkipCheckLogin标签则阻断下面的代码继续执行
            Type attrType = typeof(SkipCheckLoginAttribute);
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(attrType, false)
                ||
                filterContext.ActionDescriptor.IsDefined(attrType, false))
            {
                return;
            }

            //1.0 判断登录用户的session是否为null
            if (filterContext.HttpContext.Session[Keys.UInfo] == null)
            {
                //2.0 判断浏览器是否将免登陆的cookie发送给了服务器，如果发送了，则模拟登录
                if (filterContext.HttpContext.Request.Cookies[Keys.UID] != null)
                {
                    //2.0.1 获取浏览器发送过来的cookie中的用户id
                    string uid = filterContext.HttpContext.Request.Cookies[Keys.UID].Value;
                    //2.0.2 根据用户id去数据表sysUserInfo中查询一条数据以sysUserInfo对象实例返回
                    //获取autofac容器对象
                    IsysUserInfoServices userSer = AutofacManager.GetInstance<IsysUserInfoServices>(); //userSer的实例化交给autofac

                    int iuid = uid.AsInt();
                    var userInfo = userSer.QueryWhere(c => c.uID == iuid).FirstOrDefault();
                    if (userInfo == null)
                    {
                        //跳转到登录页面
                        ToLogin(filterContext);
                    }
                    else
                    {
                        //2.0.3 将sysUserInfo对象实例存入到Session[Keys.UInfo]中
                        filterContext.HttpContext.Session[Keys.UInfo] = userInfo;
                    }
                }
                else
                {
                    //3.0 跳转到登录页面
                    ToLogin(filterContext);
                }
            }
        }

        private static void ToLogin(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                JsonResult json = new JsonResult();
                json.Data = new { status = 2, msg = "您未登录或者登录失效" };
                json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = json;
            }
            else
            {
                ViewResult view = new ViewResult();
                view.ViewName = "/Areas/admin/Views/Login/Login.cshtml";
                filterContext.Result = view;

            }
        }
    }
}
