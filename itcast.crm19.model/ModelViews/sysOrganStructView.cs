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

    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    public partial class sysOrganStructView
    {


        public int osID { get; set; }
        [DisplayName("父级组织结构id")]
        public int osParentID { get; set; }
        [DisplayName("组织结构名称")]

        public string osName { get; set; }
        [DisplayName("组织结构类别")]
        public int osCateID { get; set; }

        public Nullable<int> osLevel { get; set; }
        [DisplayName("组织结构状态")]

        public Nullable<int> osStatus { get; set; }
        public System.DateTime osCreateTime { get; set; }
        public Nullable<int> osUpdateID { get; set; }
        public System.DateTime osUpdateTime { get; set; }






    }
}