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
    public class roominfoController : BaseController
    {

        public roominfoController(
            IroomRoomInfoServices roomRoomSer,
            IroomKeyValueServices roomKeyvalueSer,
            IroomTypeServices roomTypeSer
            )
        {
            base.roomRoomSer = roomRoomSer;
            base.roomKeyvalueSer = roomKeyvalueSer;
            base.roomTypeSer = roomTypeSer;
        }
        //
        // GET: /room/roominfo/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost, SkipCheckPermiss]

        public ActionResult getlist()
        {
            var list = roomRoomSer.QueryJoin(c => true, new string[] { "roomKeyValue", "roomType" })
                .ToList()
                .Select(c => new
                {
                    c.roomID,
                    c.roomName,
                    c.roomType.typeName,
                    c.roomKeyValue.KName,
                    roomCreator = UserManager.GetUserInfoByID(c.roomCreatorID).uRealName,
                    CreateTime = c.roomCreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();
            return Json(new { Rows = list });
        }
        [HttpGet]
        public ActionResult add()
        {

            var roomTypeDic = roomTypeSer.QueryWhere(c => true);
            SelectList roomType = new SelectList(roomTypeDic, "typeID", "typeName");
            //数据字典类别 1:客户类型 2:房间类型 3:入住类型
            var roomStatusDic = roomKeyvalueSer.QueryWhere(c => c.KType == 2);
            SelectList roomStatus = new SelectList(roomStatusDic, "KID", "KName");
            ViewBag.roomType = roomType;
            ViewBag.roomStatus = roomStatus;
            return View();
        }
        [HttpPost]
        public ActionResult add(roomRoomInfo model)
        {
            model.roomCreatorID = UserManager.LoginedUserInfo().uID;
            model.roomCreateTime = DateTime.Now;
            roomRoomSer.Add(model);
            roomRoomSer.SaveChanges();
            return WriteSuccess("新增房间成功");
        }
        [HttpGet]
        public ActionResult edit(int id)
        {
            var model = roomRoomSer.QueryWhere(c => c.roomID == id).FirstOrDefault();
            var roomTypeDic = roomTypeSer.QueryWhere(c => true);
            SelectList roomType = new SelectList(roomTypeDic, "typeID", "typeName");
            //数据字典类别 1:客户类型 2:房间类型 3:入住类型
            var roomStatusDic = roomKeyvalueSer.QueryWhere(c => c.KType == 2);
            SelectList roomStatus = new SelectList(roomStatusDic, "KID", "KName");
            ViewBag.roomType = roomType;
            ViewBag.roomStatus = roomStatus;
            return View(model);
        }
        [HttpPost]
        public ActionResult edit(int id, roomRoomInfo model)
        {
            model.roomID = id;
            roomRoomSer.Edit(model, new string[] { "roomName", "roomTypeID", "roomStatus"});
            roomRoomSer.SaveChanges();
            return WriteSuccess("修改成功");

        }


        [HttpPost]
        public ActionResult del(int id)
        {

            var currentItem = roomRoomSer.QueryWhere(c => c.roomID == id).FirstOrDefault();
            roomRoomSer.Delete(currentItem, true);
            roomRoomSer.SaveChanges();
            return WriteSuccess("删除成功");
        }

    }
}
