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
    public class typeController : BaseController
    {
        public typeController(IroomTypeServices roomTypeSer)
        {
            base.roomTypeSer = roomTypeSer;
        }
        //
        // GET: /room/type/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult getlist()
        {
            var list = roomTypeSer.QueryWhere(c => true)
                .Select(c => new
                {
                    c.typeID,
                    c.typeName,
                    c.bedNum,
                    c.price
                })
                .ToList();
            return Json(new { Rows = list });
        }
        public ActionResult add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult add(roomType model)
        {

            roomTypeSer.Add(model);
            roomTypeSer.SaveChanges();
            return WriteSuccess("新增房间类型成功");
        }
        [HttpPost]
        public ActionResult del(int id)
        {

            var currentItem = roomTypeSer.QueryWhere(c => c.typeID == id).FirstOrDefault();
            roomTypeSer.Delete(currentItem, true);
            roomTypeSer.SaveChanges();
            return WriteSuccess("删除成功");
        }


    }
}
