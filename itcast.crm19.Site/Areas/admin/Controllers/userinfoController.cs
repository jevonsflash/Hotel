using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using itcast.crm19.IServices;
using itcast.crm19.model;
using itcast.crm19.Common;
using itcast.crm19.WebHelper;
using itcast.crm19.WebHelper.Attrs;

namespace itcast.crm19.Site.Areas.admin.Controllers
{
    public class userinfoController : BaseController
    {
        public userinfoController(IsysUserInfoServices userSer, IsysRoleServices rSer, IsysUserInfo_RoleServices urSer, IsysOrganStructServices organSer)
        {
            base.userinfoSer = userSer;
            base.roleSer = rSer;
            base.userinfoRoleSer = urSer;
            base.organSer = organSer;
        }
        //
        // GET: /admin/userinfo/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult add()
        {
            //设置优先级的下拉框的数据源
            Dictionary<int, string> uGenderDic = new Dictionary<int, string>();
            uGenderDic.Add(0, "保密");
            uGenderDic.Add(1, "男");
            uGenderDic.Add(2, "女");
            SelectList uGenderSelectList = new SelectList(uGenderDic, "key", "value");
            ViewBag.gender = uGenderSelectList;

            Dictionary<int, string> uStatusDic = new Dictionary<int, string>();
            uStatusDic.Add(0, "正常");
            uStatusDic.Add(1, "停用");
            SelectList uStatusSelectList = new SelectList(uStatusDic, "key", "value");
            ViewBag.Status = uStatusSelectList;

            Dictionary<int, string> uCompanyIDDic = new Dictionary<int, string>();
            uCompanyIDDic.Add(4, "广州分公司");
            uCompanyIDDic.Add(5, "上海分公司");
            uCompanyIDDic.Add(7, "郑州分公司");

            SelectList uCompanyIDList = new SelectList(uCompanyIDDic, "key", "value");
            ViewBag.uCompanyID = uCompanyIDList;

            var list = organSer.QueryWhere(c => c.osLevel == 3 && c.osParentID == 5);
            SelectList uWorkgroupIDList = new SelectList(list, "osID", "osName");
            ViewBag.uWorkgroupID = uWorkgroupIDList;
            return View();
        }
        [HttpPost]
        public ActionResult add(sysUserInfo model)
        {
            var creator = UserManager.LoginedUserInfo().uID;
            var time = DateTime.Now;
            model.uCreateTime = time;
            model.uUpdateTime = time;
            model.uCreatorID = creator;
            model.uUpdateID = creator;
            model.uDepID = 9;
            userinfoSer.Add(model);


            //3.0 统一保存
            userinfoSer.SaveChanges();

            //4.0 响应:{status:0/1/2,msg:}
            return WriteSuccess("添加成功");
        }

        [HttpPost, SkipCheckPermiss]
        public ActionResult del(int id)
        {
            var currentUser = userinfoSer.QueryWhere(c => c.uID == id).FirstOrDefault();
            userinfoSer.Delete(currentUser, true);
            userinfoSer.SaveChanges();
            return WriteSuccess("删除成功");
        }

        [HttpGet, SkipCheckPermiss]
        public ActionResult edit(int id)
        {
            return View();
        }



        [HttpPost]
        public ActionResult getlist(FormCollection f)
        {
            //1.0 获取ligerui中的分页控件的分页参数
            int page = f["page"].AsInt();
            int pagesize = f["pagesize"].AsInt();
            int totalcount = 0;

            //1.0 调用分页方法获取所有的用户数据
            var list = userinfoSer.QueryByPage(page, pagesize, out totalcount, c => true, c => c.uID)
                .Select(
                c => new
                {
                    c.uID,
                    c.uLoginName,
                    c.uRealName,
                    c.uQQ,
                    c.uEmial,
                    c.uGender,
                    c.uTelphone,
                    c.uMobile,
                    c.uStatus,
                    c.uWorkGroupID
                }
                );

            //将分页数据获取返回，同时将总条数返回
            return Json(new { Rows = list, Total = totalcount });
        }
        [HttpGet]
        public ActionResult setRole(string id)
        {
            string[] arr = id.Split('-');
            ViewBag.WorkGroupId = arr[0];
            ViewBag.uid = arr[1];
            return View();

        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult getRoleList(FormCollection f)
        {
            int workGroupId = f["WorkGroupId"].AsInt();
            int uid = f["uid"].AsInt();
            var oldRole = userinfoRoleSer.QueryWhere(c => c.uID == uid).ToList();
            var list = roleSer.QueryWhere(c => c.eDepID == workGroupId).ToList()
                .Select(c => new
                {
                    c.rID,
                    c.rName,
                    isChecked = oldRole.Any(d => d.rID == c.rID)
                });

            return Json(new { Rows = list });
        }


        [HttpPost]
        public ActionResult setRole()
        {
            //获取参数格式： uid-rid,rid....,
            string p = Request.Form["params"];
            //拆解出数组
            string[] uidRidArr = p.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            int uid = uidRidArr[0].AsInt();
            string ridstring = uidRidArr[1]; //格式: rid,rid,.....,
            userinfoRoleSer.QueryWhere(c => c.uID == uid).ToList().ForEach(c => userinfoRoleSer.Delete(c, true));
            string[] ridArr = ridstring.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            sysUserInfo_Role model;
            foreach (var rid in ridArr)
            {
                model = new sysUserInfo_Role() { uID = uid, rID = rid.AsInt() };
                userinfoRoleSer.Add(model);
            }
            using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
            {
                //统一保存
                userinfoRoleSer.SaveChanges();
                scop.Complete();
            }
            // 返回成功提醒
            return WriteSuccess("设置用户角色完毕");
        }
    }
}
