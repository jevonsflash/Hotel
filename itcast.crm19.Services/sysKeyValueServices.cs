//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace itcast.crm19.Services
{
    using System;
    using System.Collections.Generic;
    
    using itcast.crm19.IServices;
    using itcast.crm19.IRepository;
    using itcast.crm19.model;
    public partial class sysKeyValueServices: BaseServices<sysKeyValue>,IsysKeyValueServices
    {
    
    		IsysKeyValueRepository dal;
            public sysKeyValueServices(IsysKeyValueRepository dal)
            {
                this.dal = dal;
                base.baseDal = dal;
            }
    
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
}
