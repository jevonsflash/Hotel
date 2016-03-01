using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.WebHelper.Attrs
{
    /// <summary>
    /// 表示任何的控制器和aciton方法均可以贴此标签，如果贴上来则跳过登录验证检查逻辑
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SkipCheckLoginAttribute : Attribute
    {
    }
}
