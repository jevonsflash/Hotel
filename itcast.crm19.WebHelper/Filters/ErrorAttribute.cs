using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.WebHelper.Filters
{
    using System.Web.Mvc;

    /// <summary>
    /// 全局的异常捕获过滤器
    /// </summary>
    public class ErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            try
            {
                //1.0 异常写入到日志中
                Exception exp = filterContext.Exception;
                //2.0 获取exp的详细的内部异常
                Exception innerExp = exp.InnerException != null ? exp.InnerException : exp;
                while (innerExp.InnerException != null)
                {
                    innerExp = innerExp.InnerException;
                }

                //3.0 将详细异常写入到系统日志中 :大家注意：真正的项目中使用的是日志组件,开源的有Log4Net
                //日志的记录常用的有：1、记住到文本文件中（没有统计意义的，但是量又大的）   2、记录到数据库中(出异常报表一定记录到db中)
                System.IO.File.AppendAllText(filterContext.HttpContext.Server.MapPath("/Logs/Log.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "=" + innerExp.ToString() + "\r\n");

                //4.0 判断当前请求是否为ajax请求
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    JsonResult json = new JsonResult();
                    json.Data = new { status = 1, msg = innerExp.Message };
                    json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    //将json格式字符串响应回去
                    filterContext.Result = json;

                    //告诉MVC我已经将错误变成了json返回了,你不需要再将友好的页面响应回去
                    filterContext.ExceptionHandled = true;
                }
                else //浏览器的导航请求
                {
                    //4.0 配置友好的错误页面
                    filterContext.ExceptionHandled = false;

                    base.OnException(filterContext);
                }
            }
            catch
            {
                //这个异常写入到数据库中，甚至可以写入到window的应用程序异常
            }
        }
    }
}
