using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Reflection;
using System.Transactions;
using itcast.crm19.IServices;
using itcast.crm19.model;
using itcast.crm19.WebHelper;
using itcast.crm19.Common;
using itcast.crm19.WebHelper.Attrs;

namespace itcast.crm19.Site.Areas.workflow.Controllers
{
    public class wfprocessController : BaseController
    {
        int userid;
        public wfprocessController(
            IwfProcessServices pSer,
            IwfRequestFormServices rfSer,
            IwfWorkNodesServices nodeSer,
            IwfWorkServices workSer,
            IsysUserInfo_RoleServices urSer,
            IsysRoleServices roleSer,
            IsysKeyValueServices keyvalSer,
            IwfWorkBranchServices workbranchSer
            )
        {
            base.processSer = pSer;
            base.requestformSer = rfSer;
            base.worknodesSer = nodeSer;
            base.workSer = workSer;
            base.userinfoRoleSer = urSer;
            base.roleSer = roleSer;
            base.keyvalSer = keyvalSer;
            base.workbranchSer = workbranchSer;
            this.userid = UserManager.LoginedUserInfo().uID;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult getlist(FormCollection f)
        {
            string kname = f["parms"];
            //获取当前用户角色集合

            var rids = userinfoRoleSer.QueryWhere(c => c.uID == userid).Select(c => c.rID).ToList();

            //if (kname!=null&&kname!=string.Empty)
            //{
            //    List
            //}


            var list = processSer.QueryJoin(c => rids.Contains(c.wfProcessor), new string[] { "wfRequestForm", "wfWorkNodes", "sysKeyValue" }).Select(c => new
            {
                c.wfPID,
                c.wfRequestForm.wfRFTitle,
                c.wfRequestForm.wfRFNum,
                c.wfWorkNodes.wfNodeTitle,
                c.wfRequestForm.wfRFPriority,
                c.sysKeyValue.KName,
                c.wfRFStatus
            });
            return Json(new { Rows = list });
        }

        [HttpGet]
        public ActionResult checkform(int id)
        {
            //根据id获取wfprocess实体数据
            var processModel = processSer.QueryWhere(c => c.wfPID == id).FirstOrDefault();
            int requestFormID = processModel.wfRFID;
            //根据requestFormID去wfprocess表中获取此申请单的所有明细数据
            var list = processSer.QueryWhere(c => c.wfRFID == requestFormID).ToList();
            //将id值以viewbag形式传入到视图
            ViewBag.wfpid = id;
            return View(list);
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult reject(FormCollection f)
        {
            int rid = userinfoRoleSer.QueryWhere(c => c.uID == userid).FirstOrDefault().rID;
            //获取参数
            string Description = f["wfPDescription"];
            int wfpid = f["wfpid"].AsInt();
            //更新参数wfprocess表状态，加入理由，当前uid赋予wfpext1

            var processModel = processSer.QueryWhere(c => c.wfPID == wfpid).FirstOrDefault();
            if (processModel.wfRFStatus != (int)Enums.ProcessStatusEnum.Processing)
            {
                return WriteError("请勿重复审核");
            }


            processModel.wfRFStatus = (int)Enums.ProcessStatusEnum.Reject;
            processModel.wfPExt1 = this.userid.ToString();
            processModel.wfPDescription = Description;
            processModel.fUpdateTime = DateTime.Now;



            //向wfprocess表中增加一条明细数据
            var requestFormModel = processModel.wfRequestForm;

            var lastNode = worknodesSer.QueryWhere(c => c.wfID == requestFormModel.wfID)
                .OrderByDescending(c => c.wfnOrderNo).ToList().FirstOrDefault();


            var endProcess = new wfProcess()
            {
                wfRFID = processModel.wfRFID,
                wfPDescription = "审批已经结束",
                wfProcessor = rid,
                wfRFStatus = (int)Enums.ProcessStatusEnum.Pass,
                wfnID = lastNode.wfnID,
                wfPExt1 = userid.ToString(),
                wfPExt2 = processModel.wfPID,
                fCreatorID = userid,
                fCreateTime = DateTime.Now,
                fUpdateTime = DateTime.Now
            };
            processSer.Add(endProcess);

            //更新wfReqestForm中的的状态为"拒绝"
            requestFormModel.wfRFStatus = (int)Enums.ProcessStatusEnum.Reject;

            //开启分布式事务保存
            using (TransactionScope scop = new TransactionScope())
            {
                processSer.SaveChanges();

                scop.Complete();
            }

            return WriteSuccess("审核单已经被拒绝");

        }

        [HttpPost, SkipCheckPermiss]
        public ActionResult pass(FormCollection f)
        {
            int rid = userinfoRoleSer.QueryWhere(c => c.uID == userid).FirstOrDefault().rID;
            //获取参数
            string Description = f["wfPDescription"];
            int wfpid = f["wfpid"].AsInt();
            //更新wfprocess状态，而且 uid赋值

            var processModel = processSer.QueryWhere(c => c.wfPID == wfpid).FirstOrDefault();

            if (processModel.wfRFStatus != (int)Enums.ProcessStatusEnum.Processing)
            {
                return WriteError("请勿重复审核");
            }
            processModel.wfRFStatus = (int)Enums.ProcessStatusEnum.Pass;
            processModel.wfPExt1 = this.userid.ToString();
            processModel.wfPDescription = Description;
            processModel.fUpdateTime = DateTime.Now;

            //查询下一个节点
            var nodeModel = processModel.wfWorkNodes;
            var requestFormModel = processModel.wfRequestForm;

            string bizMethod = nodeModel.wfnBizMethod;
            decimal maxNum = nodeModel.wfnMaxNum;
            decimal rfNum = requestFormModel.wfRFNum;
            //通过反射的方法
            Assembly ass = Assembly.Load("itcast.crm19.WebHelper");
            Type type = ass.GetType("itcast.crm19.WebHelper.WfBizMethods", false, true);
            MethodInfo minfo = type.GetMethod(bizMethod);
            object instanst = Activator.CreateInstance(type);
            object boolFlag = minfo.Invoke(instanst, new object[] { rfNum, maxNum });

            string token = boolFlag.ToString();
            var branchModel = workbranchSer.QueryWhere(c => c.wfnToken == token && c.wfnID == nodeModel.wfnID)
                .FirstOrDefault();
            var nextNode = branchModel.wfWorkNodes1;
            if (nextNode.wfNodeType == (int)Enums.NodeTypeEnum.EndNode)
            {
                var endProcess = new wfProcess()
                {
                    wfRFID = processModel.wfRFID,
                    wfPDescription = "审批已经结束",
                    wfProcessor = rid,
                    wfRFStatus = (int)Enums.ProcessStatusEnum.Pass,
                    wfnID = nextNode.wfnID,
                    wfPExt1 = userid.ToString(),
                    wfPExt2 = processModel.wfPID,
                    fCreatorID = userid,
                    fCreateTime = DateTime.Now,
                    fUpdateTime = DateTime.Now


                };
                processSer.Add(endProcess);
                //wfRequestform中的状态修改成通过状态
                requestFormModel.wfRFStatus = (int)Enums.ProcessStatusEnum.Pass;
            }
            else
            {
                //根据nextNode中的RoleType和当前
                int roleType = nextNode.wfnRoleType;
                int eDepID = UserManager.LoginedUserInfo().uWorkGroupID.Value;
                var nextRid = roleSer.QueryWhere(c => c.eDepID == eDepID && c.RoleType == roleType).FirstOrDefault().rID;
                var nextProcess = new wfProcess()
                {
                    wfRFID = processModel.wfRFID,
                    wfProcessor = nextRid,
                    wfRFStatus = (int)Enums.ProcessStatusEnum.Processing,
                    wfnID = nextNode.wfnID,
                    wfPExt2 = processModel.wfPID,
                    fCreatorID = userid,
                    fCreateTime = DateTime.Now,
                    fUpdateTime = DateTime.Now
                };
                processSer.Add(nextProcess);
            }
            using (TransactionScope scop = new TransactionScope())
            {
                processSer.SaveChanges();

                scop.Complete();
            }
            return WriteSuccess("申请单已经通过");
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult back(FormCollection f)
        {
            int rid = userinfoRoleSer.QueryWhere(c => c.uID == userid).FirstOrDefault().rID;
            //获取参数
            string Description = f["wfPDescription"];
            int wfpid = f["wfpid"].AsInt();
            //更新wfprocess状态，而且 uid赋值

            var processModel = processSer.QueryWhere(c => c.wfPID == wfpid).FirstOrDefault();

            if (processModel.wfRFStatus != (int)Enums.ProcessStatusEnum.Processing)
            {
                return WriteError("请勿重复审核");
            }
            var nodeModel = processModel.wfWorkNodes;

            if (nodeModel.wfnOrderNo == 2)
            {
                return WriteError("当前节点无法进行驳回操作");
            }
            processModel.wfRFStatus = (int)Enums.ProcessStatusEnum.Back;
            processModel.wfPExt1 = userid.ToString();
            processModel.wfPDescription = Description;
            processModel.fUpdateTime = DateTime.Now;

            //查询下一个节点
            var preProcess = processSer.QueryWhere(c => c.wfPID == processModel.wfPExt2).FirstOrDefault();
            preProcess.wfRFStatus = (int)Enums.ProcessStatusEnum.Processing;
            preProcess.wfPDescription = null;
            preProcess.wfPExt1 = string.Empty;
            //将preProcess状态修改为added
            processSer.Add(preProcess);

            using (TransactionScope scope = new TransactionScope())
            {
                processSer.SaveChanges();
                scope.Complete();
            }
            return WriteSuccess("申请单已驳回上级");

        }

    }
}
