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

namespace itcast.crm19.Site.Areas.admin.Controllers
{
    public class keyvalueController : BaseController
    {
        public keyvalueController(IsysKeyValueServices keyvalSer)
        {
            base.keyvalSer = keyvalSer;
        }
        //
        // GET: /admin/keyvalue/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult getlist()
        {
            var list = keyvalSer.QueryWhere(c => true)
                .Select(c => new
                {
                    c.KID,
                    c.ParentID,
                    c.Kvalue,
                    c.KName,
                    c.KType,
                    c.KRemark
                });
            return Json(new { Rows = list });
        }
        [HttpGet]
        public ActionResult add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult add(sysKeyValue model)
        {
            keyvalSer.Add(model);
            keyvalSer.SaveChanges();
            return WriteSuccess("新增数据字典成功");
        }

    }
}
