using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;
using itcast.crm19.IServices;
using itcast.crm19.WebHelper.Attrs;
using itcast.crm19.WebHelper;
using itcast.crm19.Common;
using itcast.crm19.model;
using System.Transactions;
namespace itcast.crm19.Site.Areas.admin.Controllers
{
    public class roleController : BaseController
    {

        public roleController(IsysOrganStructServices oSer, IsysRoleServices rSer, IsysMenusServices mSer, IsysFunctionServices fSer, IsysPermissListServices pSer)
        {
            base.organSer = oSer;
            base.roleSer = rSer;
            base.menuSer = mSer;
            base.funSer = fSer;
            base.permissSer = pSer;
        }
        //
        // GET: /admin/role/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, SkipCheckPermiss]
        public ActionResult getOrganTree()
        {
            var list = organSer.QueryWhere(c => true).Select(c => new
            {
                c.osID,
                c.osParentID,
                c.osName

            });
            return Json(list);
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult getlist(int id)
        {
            var list = roleSer.QueryWhere(c => c.eDepID == id).OrderBy(c => c.rSort).Select(c => new
            {
                c.rID,
                c.rName,
                c.rStatus,
                c.rSort
            });
            return Json(new { Rows = list });
        }
        [HttpGet]
        public ActionResult setPermiss(int id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public ActionResult setPermiss(FormCollection f)
        {
            int uid = UserManager.LoginedUserInfo().uID;
            //获取ajax异步对象传入的参数
            string params1 = f["params"];
            params1 = params1.Replace("m", string.Empty).Replace("f", string.Empty);
            string[] ridPermiss = params1.Split('-');
            int rid = ridPermiss[0].AsInt();
            string[] midFids = ridPermiss[1].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            permissSer.QueryWhere(c => c.rID == rid).ToList().ForEach(c => permissSer.Delete(c, true));
            string[] newArr;
            int mid;
            int fid;
            sysPermissList model;
            foreach (var item in midFids)
            {
                newArr = item.Split(',');
                mid = newArr[0].AsInt();
                fid = newArr[1].AsInt();
                model = new sysPermissList()
                {
                    rID = rid,
                    mID = mid,
                    fID = fid,
                    plCreatorID = uid,
                    plCreateTime = DateTime.Now
                };
                permissSer.Add(model);
            }
            using (TransactionScope scope = new TransactionScope())
            {
                permissSer.SaveChanges();
                scope.Complete();
            }
            return WriteSuccess("权限设置完毕");
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult getMenusAndFunctions(int id)
        {
            var oldPermissList = permissSer.QueryWhere(c => c.rID == id).ToList();
            var menuList = menuSer.QueryWhere(c => c.mStatus == (int)Enums.StatusEnum.Normal)
                .ToList()
                .Select(c => new
                {
                    text = c.mName,
                    id = "m" + c.mID,
                    pid = "m" + c.mParentID,
                    ismenus = true,
                    ischecked = oldPermissList.Any(d => d.mID == c.mID)
                }).ToList();

            var functionsList = funSer.QueryWhere(c => c.fStatus == (int)Enums.StatusEnum.Normal && c.fID > 0)
                .ToList()
                .Select(c => new
                 {
                     text = c.fName,
                     id = "f" + c.fID,
                     pid = "m" + c.mID,
                     ismenus = false,
                     ischecked = oldPermissList.Any(d => d.fID == c.fID)

                 });
            menuList.AddRange(functionsList);
            return Json(menuList);
        }
    }
}
