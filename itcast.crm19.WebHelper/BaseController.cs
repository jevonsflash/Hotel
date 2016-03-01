using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.WebHelper
{
    using System.Web.Mvc;
    using IServices;
    public class BaseController : Controller
    {
        #region 1.0 封装ajax响应的相关方法

        public ActionResult WriteError(string msg)
        {
            return Json(new { status = 1, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WriteSuccess(string msg)
        {
            return Json(new { status = 0, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WriteSuccess(string msg, object obj)
        {
            return Json(new { status = 0, msg = msg, datas = obj }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region 2.0 将当前数据库表的业务逻辑接口成员定义在此处，将来被子类构造函数实例化

        protected IsysFunctionServices funSer;
        protected IsysKeyValueServices keyvalSer;
        protected IsysMenusServices menuSer;
        protected IsysOrganStructServices organSer;
        protected IsysPermissListServices permissSer;
        protected IsysRoleServices roleSer;
        protected IsysUserInfoServices userinfoSer;
        protected IsysUserInfo_RoleServices userinfoRoleSer;
        protected IwfProcessServices processSer;
        protected IwfRequestFormServices requestformSer;
        protected IwfWorkServices workSer;
        protected IwfWorkBranchServices workbranchSer;
        protected IwfWorkNodesServices worknodesSer;
        protected IroomKeyValueServices roomKeyvalueSer;
        protected IroomRoomInfoServices roomRoomSer;
        protected IroomGuestInfoServices roomGuestSer;
        protected IroomCheckInInfoServices roomCheckSer;
        protected IroomTypeServices roomTypeSer;

        #endregion


    }
}
