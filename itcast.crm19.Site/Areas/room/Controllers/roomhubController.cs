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
    public class roomhubController : BaseController
    {

        public roomhubController(
            IroomRoomInfoServices roomRoomSer,
            IroomCheckInInfoServices roomCheckSer,
            IroomGuestInfoServices roomGuestSer,
            IroomTypeServices roomTypeSer
            )
        {
            base.roomRoomSer = roomRoomSer;
            base.roomCheckSer = roomCheckSer;
            base.roomGuestSer = roomGuestSer;
            base.roomTypeSer = roomTypeSer;
        }
        //
        // GET: /room/roomhub/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost, SkipCheckPermiss]

        public ActionResult getlist()
        {

            var list = roomRoomSer.QueryJoin(c => true, new string[] { "roomType" })
                .ToList()
                .Select(c => new
                {
                    c.roomID,
                    c.roomName,
                    c.roomType.typeName,
                    c.roomType.bedNum,
                    statusStyle = GetStatus(c.roomStatus)[0],
                    statusName = GetStatus(c.roomStatus)[1],
                    addcheckinenable = GetStatus(c.roomStatus)[2],
                    delcheckinenable = GetStatus(c.roomStatus)[3]
                }).ToList();
            return Json(list);
        }
        private string[] GetStatus(int Status)
        {
            string statusStyle = string.Empty;
            string statusName = string.Empty;
            string addCheckInEnable = "disabled";
            string delCheckInEnable = "disabled";
            switch (Status)
            {
                case 6: statusStyle = "alert alert-info"; statusName = "维护中"; break;
                case 5: statusStyle = "alert"; statusName = "清洁中"; break;
                case 7: statusStyle = "alert alert-error"; statusName = "使用中"; delCheckInEnable = string.Empty; break;
                case 4: statusStyle = "alert alert-success"; statusName = "空闲"; addCheckInEnable = string.Empty; break;
                default: statusStyle = ""; statusName = "";
                    break;
            }
            return new string[] { statusStyle, statusName, addCheckInEnable, delCheckInEnable };
        }

        [HttpGet, SkipCheckPermiss]
        public ActionResult getdetail(int id)
        {
            var roomModel = roomRoomSer.QueryWhere(c => c.roomID == id).FirstOrDefault();
            var roomTypeModel = roomModel.roomType;
            var checkInModel = roomCheckSer.QueryWhere(c => c.checkInRoomID == id).FirstOrDefault();
            if (checkInModel == null)
            {
                ViewResult newView = new ViewResult();
                newView.ViewName = "/Areas/room/Views/roomhub/getdetailsimple.cshtml";
                newView.ViewBag.roomTypeModel = roomTypeModel;
                return newView;
            }
            var guestModel = checkInModel.roomGuestInfo;
            TimeSpan duration = DateTime.Now - checkInModel.checkInTime;
            decimal totalPrice = duration.Days * roomTypeModel.price;
            ViewBag.duration = duration;
            ViewBag.totalPrice = totalPrice;
            ViewBag.guestModel = guestModel;
            ViewBag.checkInModel = checkInModel;

            ViewBag.roomTypeModel = roomTypeModel;
            return View();
        }
    }
}
