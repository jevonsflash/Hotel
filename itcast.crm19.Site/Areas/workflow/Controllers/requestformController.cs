using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Transactions;
using itcast.crm19.Common;
using itcast.crm19.IServices;
using itcast.crm19.model;
using itcast.crm19.WebHelper;
using itcast.crm19.WebHelper.Attrs;
using System.Linq.Expressions;

namespace itcast.crm19.Site.Areas.workflow.Controllers
{
    public class requestformController : BaseController
    {
        public requestformController(
            IwfRequestFormServices requestformSer,
            IwfProcessServices processSer,
            IwfWorkServices workSer,
            IsysKeyValueServices keyvalSer,
            IwfWorkNodesServices worknodesSer,
            IsysRoleServices roleSer,
            IsysUserInfo_RoleServices userinfoRoleSer
            )
        {
            base.requestformSer = requestformSer;
            base.processSer = processSer;
            base.workSer = workSer;
            base.keyvalSer = keyvalSer;
            base.worknodesSer = worknodesSer;
            base.roleSer = roleSer;
            base.userinfoRoleSer = userinfoRoleSer;
        }
        public ActionResult Index()
        {
            var list = workSer.QueryWhere(c => c.wfStatus == (int)Enums.StatusEnum.Normal).ToList();
            list.Insert(0, new wfWork() { wfID = 0, wfTitle = "请选择" });
            ViewBag.wfworks = new SelectList(list, "wfID", "wfTitle");
            return View();
        }
        public ActionResult getlist(FormCollection f)
        {
            int page = f["page"].AsInt();
            int pageSize = f["pagesize"].AsInt();
            int totalCount = 0;
            string kname = f["kname"];
            int wfid = f["wfid"].AsInt();
            //定义c的变量,类型是 wfRequestForm
            var c = Expression.Parameter(typeof(wfRequestForm), "c");
            //构造 c=>true
            Expression where = Expression.Constant(true);
            if (!kname.IsEmpty())
            {
                var knameConst = Expression.Constant(kname);
                var query1 = Expression.Call(
                Expression.Property(c, typeof(wfRequestForm).GetProperty("wfRFTitle")),
                typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), knameConst);  //c.wfRFTitle.Contains(kname) 
                where = Expression.And(where, query1); // true && c.wfRFTitle.Contains(kname) 
            }
            if (wfid > 0)
            {
                var wfidConst = Expression.Constant(wfid);  //wfid
                var wfidVer = Expression.PropertyOrField(c, "wfID");  //c.wfID
                //将c.wfID与wfid做一个相等操作 
                var query2 = Expression.Equal(wfidVer, wfidConst); // c.wfID==wfid

                where = Expression.And(where, query2);
            }
            Expression<Func<wfRequestForm, bool>> lambdaWhere;
            lambdaWhere = Expression.Lambda<Func<wfRequestForm, bool>>(where, c);

            //2.0 分页带条件查找数据
            var list = requestformSer.QueryByPage(page, pageSize, out totalCount, lambdaWhere, d => d.wfRFID)
                .Select(p => new
                {
                    p.wfRFID,
                    p.wfWork.wfTitle,
                    p.wfRFTitle,
                    p.wfRFPriority,
                    p.sysKeyValue1.KName,
                    p.wfRFRemark,
                    p.wfRFStatus
                });

            //3.0 返回以ligergrid要求的json格式
            return Json(new { Rows = list, Total = totalCount });
        }
        [HttpGet]
        public ActionResult add()
        {
            //设置工作流下拉框的数据源
            var works = workSer.QueryWhere(c => c.wfStatus == (int)Enums.StatusEnum.Normal);
            ViewBag.works = new SelectList(works, "wfID", "wfTitle");
            //设置优先级的下拉框的数据源
            var priority = keyvalSer.QueryWhere(c => c.KType == 4);
            ViewBag.priority = new SelectList(priority, "KID", "KName");
            return View();
        }
        [HttpPost]
        public ActionResult add(wfRequestForm model)
        {
            int userid = UserManager.LoginedUserInfo().uID;

            //将model插入到wfRequestForm表中
            model.wfRFStatus = (int)Enums.ProcessStatusEnum.Processing;
            model.fUpdateTime = DateTime.Now;
            model.fCreateTime = DateTime.Now;
            model.fCreatorID = userid;

            requestformSer.Add(model);
            // requestformSer.SaveChanges();

            //向wfProcess表中插入一条开始明细
            var currentUserRole = userinfoRoleSer.QueryWhere(c => c.uID == userid).FirstOrDefault();
            if (currentUserRole == null)
            {
                return WriteError("当前用户没有任何角色，请联系管理员");
            }
            var currentUserRoleID = currentUserRole.rID;
            //设置工作流的开始节点
            var firstNode = worknodesSer.QueryWhere(c => c.wfID == model.wfID && c.wfnOrderNo == 1).FirstOrDefault();
            if (firstNode == null)
            {
                return WriteError("工作流无基础节点，请联系管理员");
            }

            wfProcess firstDetils = new wfProcess()
            {
                wfRFID = model.wfRFID,
                wfProcessor = currentUserRoleID,
                wfRFStatus = (int)Enums.ProcessStatusEnum.Pass,
                wfPDescription = "申请单提交成功",
                wfnID = firstNode.wfnID,
                wfPExt1 = userid.ToString(),
                fCreateTime = DateTime.Now,
                fUpdateTime = DateTime.Now,
                fCreatorID = userid

            };
            processSer.Add(firstDetils);

            //根据model处理第二个节点
            //processSer.SaveChanges();//就是这里提示错误，那个错误是保存requestformSer出现的，怎么解决？
            var secNode = worknodesSer.QueryWhere(c => c.wfID == model.wfID && c.wfnOrderNo == 2).FirstOrDefault();
            int roleTypeID = secNode.wfnRoleType; // 当前第二个节点的审批角色类型值
            int eDepid = UserManager.LoginedUserInfo().uWorkGroupID.Value;
            int processRid = roleSer.QueryWhere(c => c.eDepID == eDepid && c.RoleType == roleTypeID).FirstOrDefault().rID; //目标表示 select RID from sysRole where eDepid= and RoleType=


            wfProcess secDetils = new wfProcess()
            {
                wfRFID = model.wfRFID,
                wfProcessor = processRid,//审批人的角色id
                wfRFStatus = (int)Enums.ProcessStatusEnum.Processing,
                wfnID = secNode.wfnID,
                wfPExt2 = firstDetils.wfPID,
                fCreateTime = DateTime.Now,
                fUpdateTime = DateTime.Now,
                fCreatorID = userid

            };
            //添加后保存
            processSer.Add(secDetils);
            using (TransactionScope scop = new TransactionScope())
            {
                processSer.SaveChanges();
                scop.Complete();
            }
            return WriteSuccess("申请单提交成功");
        }
        [HttpPost]
        public ActionResult del(int id)
        {
            var correlationWfprocess = processSer.QueryWhere(c => c.wfRFID == id).ToList();
            foreach (var item in correlationWfprocess)
            {
                processSer.Delete(item, true);
            }
            var currentItem = requestformSer.QueryWhere(c => c.wfRFID == id).FirstOrDefault();
            requestformSer.Delete(currentItem, true);

            using (TransactionScope scop = new TransactionScope())
            {
                processSer.SaveChanges();
                worknodesSer.SaveChanges();
                scop.Complete();
            }
            
            return WriteSuccess("删除成功");
        }
        [HttpGet, SkipCheckPermiss]
        public ActionResult getDetails(int id)
        {
            var list = processSer.QueryWhere(c => c.wfRFID == id).ToList();
            return View(list);
        }
    }
}
