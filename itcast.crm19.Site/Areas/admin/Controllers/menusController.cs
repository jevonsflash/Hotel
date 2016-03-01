using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace itcast.crm19.Site.Areas.admin.Controllers
{
    using IServices;
    using itcast.crm19.Common;
    using itcast.crm19.model;
    using itcast.crm19.WebHelper;
    using itcast.crm19.WebHelper.Attrs;
    using System.Web.WebPages;

    public class menusController : BaseController
    {
        public menusController(IsysMenusServices menuSer)
        {
            base.menuSer = menuSer;
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
                list = menuSer.QueryWhere(c => c.mName.Contains(kname)).Select(c => new
               {
                   c.mID,
                   c.mName,
                   c.mUrl,
                   c.mParentID,
                   c.mArea,
                   c.mController,
                   c.mAction,
                   c.mSortid,
                   c.mPicname,
                   c.mStatus
               });
            }
            else
            {
                list = menuSer.QueryWhere(c => true).Select(c => new
               {
                   c.mID,
                   c.mName,
                   c.mUrl,
                   c.mParentID,
                   c.mArea,
                   c.mController,
                   c.mAction,
                   c.mSortid,
                   c.mPicname,
                   c.mStatus
               });
            }

            return Json(new { Rows = list });//WriteSuccess():{status=0,msg=,datas=}
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = menuSer.QueryWhere(c => c.mID == id).FirstOrDefault();
            Dictionary<int, string> SelectDic = new Dictionary<int, string>();
            SelectDic.Add(0, "正常");
            SelectDic.Add(1, "停用");
            SelectList SelectList = new SelectList(SelectDic, "key", "value");
            ViewBag.selectList = SelectList;
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(int id, sysMenus model)
        {
            model.mID = id;
            model.mUpdateTime = DateTime.Now;
            //2.0 按需编辑

            menuSer.Edit(model, new string[] {
            "mName","mController","mUrl","mArea","mAction","mStatus","mSortid","mPicname"
            });

            //3.0 统一保存
            menuSer.SaveChanges();

            //4.0 响应:{status:0/1/2,msg:}
            return WriteSuccess("编辑成功");
        }
        [HttpGet]
        public ActionResult Add()
        {
            Dictionary<int, string> mStatusDic = new Dictionary<int, string>();
            mStatusDic.Add(0, "正常");
            mStatusDic.Add(1, "停用");
            SelectList mStatus = new SelectList(mStatusDic, "key", "value");
            //菜单级别 0:根菜单 1一级菜单 2:二级菜单
            Dictionary<int, string> mLevelDic = new Dictionary<int, string>();
            mLevelDic.Add(0, "根菜单");
            mLevelDic.Add(1, "一级菜单");
            mLevelDic.Add(2, "二级菜单");
            SelectList mLevel = new SelectList(mLevelDic, "key", "value");
            var mParentIDDic = menuSer.QueryWhere(c =>c.mLevel==1 &&c.mStatus == (int)Enums.StatusEnum.Normal);
            SelectList mParentID = new SelectList(mParentIDDic, "mID", "mName");
            ViewBag.mStatus = mStatus;
            ViewBag.mLevel = mLevel;
            ViewBag.mParentID = mParentID;
            return View();
        }
        [HttpPost]
        public ActionResult Add(sysMenus model)
        {
            model.mExp2 = 1;
            model.mCreatorID = UserManager.LoginedUserInfo().uID;
            model.mCreateTime = DateTime.Now;
            model.mUpdateTime = DateTime.Now;
            menuSer.Add(model);


            //3.0 统一保存
            menuSer.SaveChanges();

            //4.0 响应:{status:0/1/2,msg:}
            return WriteSuccess("添加成功");
        }

    }
}
