using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.model
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class LoginInfo
    {
        [DisplayName("验证码"), Required(ErrorMessage = "验证码非空")]
        public string Vcode { get; set; }
        [DisplayName("用户名"), Required(ErrorMessage = "用户名非空")]
        public string uLoginName { get; set; }
        [DisplayName("密码"), Required(ErrorMessage = "密码非空")]
        public string uLoginPWD { get; set; }
        [DisplayName("3天免登录")]
        public bool isReMember { get; set; }
    }
}
