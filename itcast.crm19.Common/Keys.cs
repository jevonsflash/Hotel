using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.Common
{
    public class Keys
    {
        public const string Vcode = "Vcode";
        public const string UInfo = "UInfo";
        /// <summary>
        /// 由于此key是给cookie使用的，那么为了防止多个系统的cookie键相同，必须加项目的前缀
        /// </summary>
        public const string UID = "Crm19Uid";
        public const string autofac = "autofac";
    }
}
