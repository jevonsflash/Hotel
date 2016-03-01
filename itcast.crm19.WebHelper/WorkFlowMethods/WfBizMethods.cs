using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.WebHelper
{
    public class WfBizMethods
    {
        [DisplayName("大于")]
        public bool Gt(decimal requestFormNum, decimal maxNum)
        {
            return requestFormNum > maxNum;
        }
        [DisplayName("大于等于")]
        public bool Geq(decimal requestFormNum, decimal maxNum)
        {
            return requestFormNum >= maxNum;
        }
        [DisplayName("小于")]
        public bool Lt(decimal requestFormNum, decimal maxNum)
        {
            return requestFormNum < maxNum;
        }
        [DisplayName("小于等于")]
        public bool Leq(decimal requestFormNum, decimal maxNum)
        {
            return requestFormNum <= maxNum;
        }
        [DisplayName("等于")]
        public bool Eq(decimal requestFormNum, decimal maxNum)
        {
            return requestFormNum == maxNum;
        }
    }
}
