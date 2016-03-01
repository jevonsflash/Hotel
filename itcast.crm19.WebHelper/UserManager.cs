
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.WebHelper
{
    using itcast.crm19.Common;
    using itcast.crm19.model;
    using System.Web;
    using IServices;

    public class UserManager
    {
        /// <summary>
        /// 获得当前登录用户
        /// </summary>
        /// <returns></returns>
        public static sysUserInfo LoginedUserInfo()
        {
            if (HttpContext.Current.Session[Keys.UInfo] != null)
            {
                return HttpContext.Current.Session[Keys.UInfo] as sysUserInfo;
            }
            return new sysUserInfo();
        }
        /// <summary>
        /// 根据指定id获得用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static sysUserInfo GetUserInfoByID(int userId)
        {
            var userSer = AutofacManager.GetInstance<IsysUserInfoServices>();
            var userInfo = userSer.QueryWhere(c => c.uID == userId).FirstOrDefault();
            return userInfo;
        }
        /// <summary>
        /// 根据指定roleId获得用户name
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        public static List<string> GetNameByRoleId(int rid)
        {
            var userRoleSer = AutofacManager.GetInstance<IsysUserInfo_RoleServices>();
            List<string> names = userRoleSer.QueryWhere(c => c.rID == rid).Select(c => c.sysUserInfo.uRealName).ToList();
            return names;
        }
    }
}
