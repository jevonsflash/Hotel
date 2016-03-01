using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using itcast.crm19.IServices;
using itcast.crm19.model;
using itcast.crm19.Common;
using itcast.crm19.WebHelper;
using itcast.crm19.WebHelper.Attrs;
namespace itcast.crm19.Site.Areas.room.Controllers
{
    public class guestinfoController : BaseController
    {
        public guestinfoController(
            IroomGuestInfoServices roomGuestSer,
            IroomKeyValueServices roomKeyvalueSer
            )
        {
            base.roomGuestSer = roomGuestSer;
            base.roomKeyvalueSer = roomKeyvalueSer;
        }

        //
        // GET: /room/guestinfo/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult getlist()
        {
            var list = roomGuestSer.QueryJoin(c => true, new string[] { "roomKeyValue" })
                .ToList()
                .Select(c => new
                {
                    c.guestID,
                    c.guestName,
                    c.guestPhone,
                    c.roomKeyValue.KName,
                    CreateTime = c.guestCreateTime.ToString("yyyy-MM-dd hh:mm:ss")
                }).ToList();
            return Json(new { Rows = list });
        }
        [HttpGet]
        public ActionResult add()
        {
            var guestStatusDic = roomKeyvalueSer.QueryWhere(c => c.KType == 1);
            SelectList guestStatus = new SelectList(guestStatusDic, "KID", "KName");
            ViewBag.guestStatus = guestStatus;
            return View();
        }
        [HttpPost]
        public ActionResult add(roomGuestInfo model)
        {
            model.guestCreatorID = UserManager.LoginedUserInfo().uID;
            model.guestCreateTime = DateTime.Now;
            roomGuestSer.Add(model);
            roomGuestSer.SaveChanges();
            return WriteSuccess("新增客户成功");
        }

        [HttpGet]
        public ActionResult edit(int id)
        {
            var model = roomGuestSer.QueryWhere(c => c.guestID == id).FirstOrDefault();
            var guestStatusDic = roomKeyvalueSer.QueryWhere(c => c.KType == 1);
            SelectList guestStatus = new SelectList(guestStatusDic, "KID", "KName");
            ViewBag.guestStatus = guestStatus;
            return View(model);
        }
        [HttpPost]
        public ActionResult edit(int id, roomGuestInfo model)
        {
            model.guestID = id;
            roomGuestSer.Edit(model, new string[] { "guestName", "guestPhone", "guestStatus" });
            roomGuestSer.SaveChanges();
            return WriteSuccess("修改成功");

        }

        [HttpPost]
        public ActionResult del(int id)
        {

            var currentItem = roomGuestSer.QueryWhere(c => c.guestID == id).FirstOrDefault();
            roomGuestSer.Delete(currentItem, true);
            roomGuestSer.SaveChanges();
            return WriteSuccess("删除成功");
        }

    }
}
