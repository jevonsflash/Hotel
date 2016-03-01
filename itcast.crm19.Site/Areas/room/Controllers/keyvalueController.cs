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
    public class keyvalueController : BaseController
    {

        public keyvalueController(IroomKeyValueServices roomKeyvalueSer)
        {
            base.roomKeyvalueSer = roomKeyvalueSer;
        }
        //
        // GET: /room/keyvalue/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult getlist()
        {
            var list = roomKeyvalueSer.QueryWhere(c => true)
                .Select(c => new
                {
                    c.KID,
                    c.Kvalue,
                    c.KName,
                    c.KType
                })
                .ToList();
            return Json(new { Rows = list });
        }
        [HttpGet]
        public ActionResult add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult add(roomKeyValue model)
        {
            roomKeyvalueSer.Add(model);
            roomKeyvalueSer.SaveChanges();
            return WriteSuccess("新增数据字典成功");
        }
        [HttpPost]
        public ActionResult del(int id)
        {
            var currentItem = roomKeyvalueSer.QueryWhere(c => c.KID == id).FirstOrDefault();
            roomKeyvalueSer.Delete(currentItem, true);
            roomKeyvalueSer.SaveChanges();
            return WriteSuccess("删除成功");
        }

    }
}
