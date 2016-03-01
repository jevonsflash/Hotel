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
using System.Transactions;
namespace itcast.crm19.Site.Areas.room.Controllers
{
    public class checkininfoController : BaseController
    {
        public checkininfoController(
            IroomCheckInInfoServices roomCheckSer,
            IroomRoomInfoServices roomRoomSer,
            IroomGuestInfoServices roomGuestSer,
            IroomKeyValueServices roomKeyvalueSer
            )
        {
            base.roomCheckSer = roomCheckSer;
            base.roomRoomSer = roomRoomSer;
            base.roomGuestSer = roomGuestSer;
            base.roomKeyvalueSer = roomKeyvalueSer;
        }
        //
        // GET: /room/checkininfo/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult getlist()
        {
            var list = roomCheckSer.QueryWhere(c => true)
                .ToList()
                .Select(c => new
                {
                    c.checkInID,
                    checkInRoom = c.roomRoomInfo.roomName,
                    checkInGuest = c.roomGuestInfo.guestName,
                    Status = c.roomKeyValue.KName,
                    checkInTime = c.checkInTime.ToString("yyyy-MM-dd hh:mm:ss"),
                    checkInCreator = UserManager.GetUserInfoByID(c.checkInCreatorID).uRealName,
                    c.checkInDeposit,
                    c.checkInRemark
                });

            return Json(new { Rows = list });
        }
        [HttpGet]
        public ActionResult add(int checkInRoomArg, int checkInGuestArg)
        {
            roomCheckInInfo model = new roomCheckInInfo();
            model.checkInRoomID = checkInRoomArg;
            model.checkInGuestID = checkInGuestArg;

            var checkInRoomDic = roomRoomSer.QueryWhere(c => true).Select(c => new { c.roomID, c.roomName }).ToList();
            checkInRoomDic.Insert(0, new { roomID = -1, roomName = "请选择" });
            SelectList checkInRoom = new SelectList(checkInRoomDic, "roomID", "roomName");
            var checkInGuestDic = roomGuestSer.QueryWhere(c => true).Select(c => new { c.guestID, c.guestName }).ToList();
            checkInGuestDic.Insert(0, new { guestID = -1, guestName = "请选择" });
            SelectList checkInGuest = new SelectList(checkInGuestDic, "guestID", "guestName");
            var checkInStatusDic = roomKeyvalueSer.QueryWhere(c => c.KType == 3).ToList();
            SelectList checkInStatus = new SelectList(checkInStatusDic, "KID", "KName");
            ViewBag.checkInRoom = checkInRoom;
            ViewBag.checkInGuest = checkInGuest;
            ViewBag.checkInStatus = checkInStatus;
            return View(model);
        }
        [HttpPost]
        public ActionResult add(roomCheckInInfo model)
        {
            if (model.checkInRoomID == -1)
            {
                return WriteError("请选择入住客房");
            }
            if (model.checkInGuestID == -1)
            {
                return WriteError("请选择入住客户");
            }

            model.checkInCreatorID = UserManager.LoginedUserInfo().uID;
            model.checkOutTime = DateTime.Now;
            roomCheckSer.Add(model);

            var roomModel = roomRoomSer.QueryWhere(c => c.roomID == model.checkInRoomID).FirstOrDefault();
            if (roomModel.roomStatus == 5)
            {
                return WriteError("此房间正在清洁，请在\"客房管理\"中解除此状态方可入住");
            }
            if (roomModel.roomStatus == 6)
            {
                return WriteError("此房间正在维护，请在\"客房管理\"中解除此状态方可入住");
            }

            if (roomModel.roomStatus == 7)
            {
                return WriteError("此房间已被占用");
            }
            roomModel.roomStatus = 7;

            using (TransactionScope scope = new TransactionScope())
            {
                roomRoomSer.SaveChanges();
                roomCheckSer.SaveChanges();
                scope.Complete();

            }
            return WriteSuccess("新增成功");
        }
        [HttpGet]
        public ActionResult edit(int id)
        {
            var model = roomCheckSer.QueryWhere(c => c.checkInID == id).FirstOrDefault();
            var checkInRoomDic = roomRoomSer.QueryWhere(c => true).Select(c => new { c.roomID, c.roomName }).ToList();
            checkInRoomDic.Insert(0, new { roomID = -1, roomName = "请选择" });
            SelectList checkInRoom = new SelectList(checkInRoomDic, "roomID", "roomName");
            var checkInGuestDic = roomGuestSer.QueryWhere(c => true).Select(c => new { c.guestID, c.guestName }).ToList();
            checkInGuestDic.Insert(0, new { guestID = -1, guestName = "请选择" });
            SelectList checkInGuest = new SelectList(checkInGuestDic, "guestID", "guestName");
            var checkInStatusDic = roomKeyvalueSer.QueryWhere(c => c.KType == 3).ToList();
            SelectList checkInStatus = new SelectList(checkInStatusDic, "KID", "KName");
            ViewBag.checkInRoom = checkInRoom;
            ViewBag.checkInGuest = checkInGuest;
            ViewBag.checkInStatus = checkInStatus;
            return View(model);
        }
        [HttpPost]
        public ActionResult edit(int id, roomCheckInInfo model)
        {
            model.checkInID = id;
            roomCheckSer.Edit(model, new string[] { "checkInRoomID", "checkInGuestID", "checkInStatus", "checkInTime", "checkInDeposit" });
            roomCheckSer.SaveChanges();
            return WriteSuccess("修改成功");

        }


        [HttpPost]
        public ActionResult del(int id)
        {

            var currentItem = roomCheckSer.QueryWhere(c => c.checkInID == id).FirstOrDefault();
            roomCheckSer.Delete(currentItem, true);
            roomCheckSer.SaveChanges();
            return WriteSuccess("删除成功");
        }

        [HttpGet,SkipCheckPermiss]
        public ActionResult checkOut(int id)
        {
            var checkInModel = roomCheckSer.QueryWhere(c => c.checkInRoomID == id).FirstOrDefault();
            var roomTypeModel = checkInModel.roomRoomInfo.roomType;
            var guestModel = checkInModel.roomGuestInfo;
            TimeSpan duration = DateTime.Now - checkInModel.checkInTime;
            decimal totalPrice = duration.Days * roomTypeModel.price;
            ViewBag.checkInRoomID = id;
            ViewBag.guestName = guestModel.guestName;
            ViewBag.duration = duration;
            ViewBag.totalPrice = totalPrice;
            return View(checkInModel);
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult checkOutAction(int id)
        {
            var roomModel = roomRoomSer.QueryWhere(c => c.roomID == id).FirstOrDefault();
            roomModel.roomStatus = 4; 
            var model = roomCheckSer.QueryWhere(c => c.checkInRoomID == roomModel.roomID).FirstOrDefault();
            model.checkInStatus = 11;

            using (TransactionScope scope = new TransactionScope())
            {
                roomRoomSer.SaveChanges();
                roomCheckSer.SaveChanges();
                scope.Complete();

            }
            return WriteSuccess("退房成功");

        }
    }
}
