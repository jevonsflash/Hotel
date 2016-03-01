using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace itcast.crm19.Site.Areas.admin.Controllers
{
    using IServices;
    using itcast.crm19.model;
    using itcast.crm19.WebHelper;
    using itcast.crm19.WebHelper.Attrs;
    using System.Web.WebPages;
    public class OrganController : BaseController
    {


        public OrganController(IsysOrganStructServices organSer, IsysKeyValueServices keyvalSer)
        {
            base.organSer = organSer;
            base.keyvalSer = keyvalSer;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, SkipCheckPermiss]
        public ActionResult getlist(FormCollection forms)
        {
            string kname = forms["kname"];
            object list;
            if (!kname.IsEmpty())
            {
                list = organSer.QueryJoin(c => c.osName.Contains(kname), new string[] { "sysKeyValue" })
                    .Select(c => new
                    {
                        c.osID,
                        c.osParentID,
                        c.osName,
                        c.sysKeyValue.KName,
                        c.osStatus,
                    });

            }
            else
            {
                list = organSer.QueryJoin(c => true, new string[] { "sysKeyValue" })
                    .Select(c => new
                    {
                        c.osID,
                        c.osParentID,
                        c.osName,
                        c.sysKeyValue.KName,
                        c.osStatus,
                    });
            }

            return Json(new { Rows = list });
        }
        [HttpGet]
        public ActionResult add()
        {
            //设置优先级的下拉框的数据源

            var osCateID = keyvalSer.QueryWhere(c => c.KType == 1);
            ViewBag.osCateID = new SelectList(osCateID, "KID", "KName");

            Dictionary<int, string> SelectDic = new Dictionary<int, string>();
            SelectDic.Add(0, "正常");
            SelectDic.Add(1, "停用");
            SelectList SelectList = new SelectList(SelectDic, "key", "value");
            ViewBag.osStatus = SelectList;

            return View();
        }
        [HttpPost]
        public ActionResult add(sysOrganStruct model)
        {
            model.osCreateTime = DateTime.Now;
            model.osUpdateID = UserManager.LoginedUserInfo().uID;
            model.osUpdateTime = DateTime.Now;
            organSer.Add(model);


            //3.0 统一保存
            organSer.SaveChanges();

            //4.0 响应:{status:0/1/2,msg:}
            return WriteSuccess("添加成功");
        }

        [HttpGet, SkipCheckPermiss]
        public ActionResult edit(int id)
        {
            return View();
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult del(int id)
        {
            return new EmptyResult();
        }


    }
}
