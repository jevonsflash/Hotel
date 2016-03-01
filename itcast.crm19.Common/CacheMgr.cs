using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.Common
{
    using System.Web;

    public class CacheMgr
    {
        /// <summary>
        /// 永久缓存，当IIS重启以后缓存失效
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetCache(string key, object value)
        {
            HttpRuntime.Cache[key] = value;
        }

        /// <summary>
        /// 从缓存中获取一个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetData<T>(string key)
        {
            return (T)HttpRuntime.Cache[key];
        }
    }
}
