using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.WebHelper.Attrs
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SkipCheckPermissAttribute : Attribute
    {
    }
}
