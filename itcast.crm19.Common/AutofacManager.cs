using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.Common
{
    using Autofac;
    public class AutofacManager
    {
        public static T GetInstance<T>()
        {
            //1.0 从缓存中得到autofac的容器对象
            var c = CacheMgr.GetData<IContainer>(Keys.autofac);

            //2.0 根据autofac容器对象获取T对应的实现类的对象实例
            return c.Resolve<T>();
        }
    }
}
