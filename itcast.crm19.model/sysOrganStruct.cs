//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace itcast.crm19.model
{
    using System;
    using System.Collections.Generic;
    
    public partial class sysOrganStruct
    {
        public sysOrganStruct() 
        {
            this.sysRole = new HashSet<sysRole>();
            this.sysUserInfo = new HashSet<sysUserInfo>();
            this.sysUserInfo1 = new HashSet<sysUserInfo>();
            this.sysUserInfo2 = new HashSet<sysUserInfo>();
        }
    
        public int osID { get; set; }
        public int osParentID { get; set; }
        public string osName { get; set; }
        public int osCateID { get; set; }
        public Nullable<int> osLevel { get; set; }
        public Nullable<int> osStatus { get; set; }
        public System.DateTime osCreateTime { get; set; }
        public Nullable<int> osUpdateID { get; set; }
        public System.DateTime osUpdateTime { get; set; }
    
        public virtual sysKeyValue sysKeyValue { get; set; }
        public virtual ICollection<sysRole> sysRole { get; set; }
        public virtual ICollection<sysUserInfo> sysUserInfo { get; set; }
        public virtual ICollection<sysUserInfo> sysUserInfo1 { get; set; }
        public virtual ICollection<sysUserInfo> sysUserInfo2 { get; set; }
    }
}
