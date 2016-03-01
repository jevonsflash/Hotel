using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using itcast.crm19.IServices;
using itcast.crm19.Common;
using itcast.crm19.model;
using itcast.crm19.WebHelper;
using itcast.crm19.WebHelper.Attrs;
using System.Reflection;
using System.ComponentModel;
using System.Transactions;
namespace itcast.crm19.Site.Areas.workflow.Controllers
{
    public class wfworkController : BaseController
    {
        public wfworkController(IwfWorkServices wSer,
            IwfWorkNodesServices nodeSer,
            IsysKeyValueServices keyvalSer,
            IwfWorkBranchServices workbranchSer
            )
        {
            base.workSer = wSer;
            base.worknodesSer = nodeSer;
            base.keyvalSer = keyvalSer;
            base.workbranchSer = workbranchSer;
        }
        //
        // GET: /workflow/wfwork/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult getlist(FormCollection f)
        {
            int pagesize = f["pageSize"].AsInt();
            int pageindex = f["page"].AsInt();
            int totalcount = 0;
            string kname = f["kname"].ToString();
            object list;
            if (kname.IsEmpty())
            {
                list = workSer.QueryByPage(pageindex, pagesize, out totalcount, c => c.wfStatus == (int)Enums.StatusEnum.Normal, c => c.wfID)
                .Select(c => new
                {
                    c.wfID,
                    c.wfTitle,
                    c.wfRemark
                });
            }
            else
            {
                list = workSer.QueryByPage(
                pageindex,
                pagesize,
                out totalcount,
                c => c.wfStatus == (int)Enums.StatusEnum.Normal
                && (c.wfTitle.Contains(kname) || c.wfRemark.Contains(kname)),
                c => c.wfID)
                .Select(c => new
                {
                    c.wfID,
                    c.wfTitle,
                    c.wfRemark
                });
            }
            return Json(new { Rows = list, Total = totalcount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult setNodes(int id)
        {
            int wfID = id;
            ViewBag.wfID = wfID;
            return View();
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult getNodes(int id)
        {
            var list = worknodesSer.QueryJoin(c => c.wfID == id, new string[] { "sysKeyValue" })
                .ToList()
                .Select(c => new
                {
                    c.wfnID,
                    c.wfNodeTitle,
                    c.wfnOrderNo,
                    c.wfNodeType,
                    NodeTypeName = c.sysKeyValue.KName,
                    RoleTypeName = c.sysKeyValue1.KName,
                    BizMethod = c.wfnBizMethod


                }).OrderBy(c => c.wfnOrderNo);
            return Json(new { Rows = list });
        }
        [HttpGet, SkipCheckPermiss]
        public ActionResult AddNode(int id)
        {
            SetKeyValue();
            SetMethods();
            return View();
        }

        [HttpPost, SkipCheckPermiss]
        public ActionResult AddNode(int id, wfWorkNodes model)
        {
            int userid = UserManager.LoginedUserInfo().uID;
            model.wfID = id;
            model.fCreateTime = DateTime.Now;
            model.fUpdateTime = DateTime.Now;
            model.fCreatorID = userid;
            int nodeCount = worknodesSer.QueryWhere(c => c.wfID == id).Count();
            model.wfnOrderNo = nodeCount > 0 ? ++nodeCount : 1;

            worknodesSer.Add(model);
            worknodesSer.SaveChanges();
            return WriteSuccess("新增成功");
        }
        [HttpGet, SkipCheckPermiss]
        public ActionResult EditNode(int id)
        {
            var model = worknodesSer.QueryWhere(c => c.wfnID == id).FirstOrDefault();
            SetKeyValue();
            SetMethods();
            return View(model);
        }

        [HttpPost, SkipCheckPermiss]
        public ActionResult EditNode(int id, wfWorkNodes model)
        {
            model.wfnID = id;
            worknodesSer.Edit(model, new string[] { "wfNodeTitle", "wfNodeType", "wfnRoleType", "wfnBizMethod", "wfnMaxNum" });
            worknodesSer.SaveChanges();
            return WriteSuccess("修改成功");
        }


        [HttpPost, SkipCheckPermiss]
        public ActionResult up(int id)
        {
            var currentNode = worknodesSer.QueryWhere(c => c.wfnID == id).FirstOrDefault();
            if (currentNode == null)
            {
                return WriteError("节点不存在");
            }
            if (currentNode.wfNodeType == (int)Enums.NodeTypeEnum.EndNode)
            {
                return WriteError("结束节点无法上移");
            }
            var wfID = currentNode.wfID;
            var preNodeSortID = currentNode.wfnOrderNo - 1;
            var preNode = worknodesSer.QueryWhere(c => c.wfnOrderNo == preNodeSortID && c.wfID == wfID).FirstOrDefault();
            if (preNode == null)
            {
                return WriteError("已经为头节点");
            }
            if (preNode.wfNodeType == (int)Enums.NodeTypeEnum.BeginNode)
            {
                return WriteError("无法上移至开始节点之前");
            }
            currentNode.wfnOrderNo--;
            preNode.wfnOrderNo++;
            using (TransactionScope scop = new TransactionScope())
            {
                worknodesSer.SaveChanges();
                scop.Complete();
            }
            return WriteSuccess("移动成功");
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult down(int id)
        {
            var currentNode = worknodesSer.QueryWhere(c => c.wfnID == id).FirstOrDefault();
            if (currentNode == null)
            {
                return WriteError("节点不存在");
            }
            if (currentNode.wfNodeType == (int)Enums.NodeTypeEnum.BeginNode)
            {
                return WriteError("开始节点无法下移");
            }
            int wfID = currentNode.wfID;
            int nextNodeSortID = currentNode.wfnOrderNo + 1;
            var nextNode = worknodesSer.QueryWhere(c => c.wfnOrderNo == nextNodeSortID && c.wfID == wfID).FirstOrDefault();
            if (nextNode == null)
            {
                return WriteError("已经为尾节点");

            }
            if (nextNode.wfNodeType == (int)Enums.NodeTypeEnum.EndNode)
            {
                return WriteError("无法下移至结束节点之后");
            }
            currentNode.wfnOrderNo++;
            nextNode.wfnOrderNo--;

            //4.0 统一保存
            using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
            {
                worknodesSer.SaveChanges();
                scop.Complete();
            }
            return WriteSuccess("移动成功");

        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult delNode(int id)
        {
            var currentNode = worknodesSer.QueryWhere(c => c.wfnID == id).FirstOrDefault();
            worknodesSer.Delete(currentNode, true);
            int wfID = currentNode.wfID;
            int curentNodeSortID = currentNode.wfnOrderNo;
            var nodes = worknodesSer.QueryWhere(c => c.wfnOrderNo > curentNodeSortID && c.wfID == wfID).ToList();
            nodes.ForEach(c => c.wfnOrderNo--);

            using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
            {
                worknodesSer.SaveChanges();
                scop.Complete();
            }
            return WriteSuccess("删除成功");
        }
        [HttpGet, SkipCheckPermiss]
        public ActionResult setBranch(int id)
        {
            ViewBag.wfnid = id;
            return View();
        }
        [HttpPost, SkipCheckPermiss]
        public ActionResult getBranchList(int id)
        {
            var list2 = workbranchSer.QueryWhere(c => c.wfnID == id).ToList();

            var list = workbranchSer.QueryJoin(c => c.wfnID == id, new string[] { "wfWorkNodes" })
                .ToList()
                .Select(c => new
            {
                c.wfbID,
                c.wfnToken,
                c.wfNodeID,
                nodeTo = c.wfWorkNodes1.wfNodeTitle,
                nodeIn = c.wfWorkNodes.wfNodeTitle,
            });
            return Json(new { Rows = list });
        }
        private void SetKeyValue()
        {
            var list = keyvalSer.QueryWhere(c => c.KType == 3 || c.KType == 2)
                .ToList();
            var nodeTypes = list.Where(c => c.KType == 3);
            var roleTypes = list.Where(c => c.KType == 2);
            ViewBag.nodetypes = new SelectList(nodeTypes, "KID", "KName");
            ViewBag.roletypes = new SelectList(roleTypes, "KID", "KName");


        }
        private void SetMethods()
        {
            Assembly ass = Assembly.Load("itcast.crm19.WebHelper");
            Type type = ass.GetType("itcast.crm19.WebHelper.WfBizMethods", false, true);
            MethodInfo[] methodInfos = type.GetMethods();
            Dictionary<string, string> optlist = new Dictionary<string, string>();
            if (methodInfos != null && methodInfos.Any())
            {
                foreach (MethodInfo item in methodInfos)
                {
                    var attr = item.GetCustomAttribute<DisplayNameAttribute>();
                    if (attr != null)
                    {
                        optlist.Add(item.Name, attr.DisplayName);
                    }
                }
            }
            ViewBag.methods = new SelectList(optlist, "key", "value");
        }
    }
}
