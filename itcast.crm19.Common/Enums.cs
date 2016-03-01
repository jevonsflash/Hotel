using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.Common
{
    public class Enums
    {
        public enum StatusEnum
        {
            Normal, Stop
        }
        public enum NodeTypeEnum
        {
            BeginNode = 34,
            ProcessNode = 35,
            EndNode = 36
        }
        public enum ProcessStatusEnum
        {
            Processing = 40,//处理中
            Back = 41,      //驳回
            Reject = 42,    //拒绝
            Pass = 43       //通过
        }
    }
}
