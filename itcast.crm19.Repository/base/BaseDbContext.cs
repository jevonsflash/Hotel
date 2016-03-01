using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.Repository
{
    using System.Data.Entity;

    /// <summary>
    /// 自定义EF上下文容器类
    /// </summary>
    public class BaseDbContext : DbContext
    {
        //初始化ado.net连接以及初始化EF容器对象
        public BaseDbContext() : base("name=JKCRMEntities") { }

      
    }
}
