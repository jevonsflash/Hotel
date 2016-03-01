using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace itcast.crm19.Site.App_Start
{
    using Autofac;
    using Autofac.Integration.Mvc;
    using itcast.crm19.Common;
    using System.Reflection;
    using System.Web.Mvc;

    /// <summary>
    /// 负责调用autofac.dll注册dal层和bll层的所有类的对象实例以接口的形式存储在内存中
    /// </summary>
    public class AutoFacConfig
    {
        public static void Register()
        {
            //1.0 创建一个autofac的容器创建者对象
            var builder = new ContainerBuilder();

            //2.0 告诉autofac控制器类所存储的程序集是谁
            Assembly controllerAss = Assembly.Load("itcast.crm19.Site");
            builder.RegisterControllers(controllerAss);


            //3.0 将仓储层中的所有的类实例化以其接口的形式存储起来
            Assembly dalAss = Assembly.Load("itcast.crm19.Repository");
            builder.RegisterTypes(dalAss.GetTypes()).AsImplementedInterfaces();

            //4.0 将业务逻辑层中的所有的类实例化以其接口的形式存储起来
            Assembly bllAss = Assembly.Load("itcast.crm19.Services");
            builder.RegisterTypes(bllAss.GetTypes()).AsImplementedInterfaces();

            //5.0 告诉MVC底层控制器的对象创建工作被autofac替代
            //5.0.1 创建一个真正的autofac工作容器
            var c = builder.Build();
            //根据某个接口从c中获取此接口的实现类的对象实例
            // IServices.IsysUserInfoServices userSer = c.Resolve<IServices.IsysUserInfoServices>(); // new sysUserInfoServices(); 
            //将c利用HttpRunTime.Cache 进行缓存
            CacheMgr.SetCache(Keys.autofac, c);

            //5.0.2 将auto发出工作容器替换MVC底层 
            System.Web.Mvc.DependencyResolver.SetResolver(new AutofacDependencyResolver(c));
        }
    }
}