using itcast.crm19.Common;
using itcast.crm19.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace itcast.crm19.Site.Areas.admin.Controllers
{
    using System.Drawing;
    using IServices;
    using itcast.crm19.WebHelper.Attrs;
    using itcast.crm19.WebHelper;

    [SkipCheckLogin]
    [SkipCheckPermiss]
    public class LoginController : BaseController
    {
        public LoginController(IsysUserInfoServices userSer)
        {
            base.userinfoSer = userSer;
        }

        [HttpGet]
        public ActionResult Login()
        {
            LoginInfo model = new LoginInfo() { uLoginName = "admin", uLoginPWD = "123456" };
            //如何获取浏览器发送过来的cookie
            if (Request.Cookies[Keys.UID] != null)
            {
                model.isReMember = true;
            }

            return View(model);
        }

        /// <summary>
        /// 异步请求
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginInfo model)
        {
            //throw new Exception("登录ajax请求异常");
            //1.0 检查验证码的合法性
            string vcodeFromSession = string.Empty;
            if (Session[Keys.Vcode] != null)
            {
                vcodeFromSession = Session[Keys.Vcode].ToString();
            }
            if (vcodeFromSession.Equals(model.Vcode, StringComparison.OrdinalIgnoreCase) == false)
            {
                //return Json(new { status = 1, msg = "验证码不合法" });
                return WriteError("验证码不合法");
            }

            //2.0 检查用户名和密码的合法性
            string md5Pwd = Kits.MD5Entry(model.uLoginPWD);
            var userInfo = userinfoSer.QueryWhere(c => c.uLoginName == model.uLoginName).FirstOrDefault();
            if (userInfo == null)
            {
                // return Json(new { status = 1, msg = "用户名错误" });
                return WriteError("用户名错误");
            }

            if (userInfo.uLoginPWD != md5Pwd)
            {
                //return Json(new { status = 1, msg = "密码错误" });
                return WriteError("密码错误");
            }

            //将用户实体写入session
            Session[Keys.UInfo] = userInfo;

            //3.0 判断是否免登陆
            //if(true) 写cookie else  清除cookie
            if (model.isReMember) //用户勾选了免登陆
            {
                //向cookie中存储当前登录用户的主键
                HttpCookie cookie = new HttpCookie(Keys.UID, userInfo.uID.ToString());
                cookie.Expires = DateTime.Now.AddDays(3);
                Response.Cookies.Add(cookie);
            }
            else
            {
                //清除cookie
                HttpCookie cookie = new HttpCookie(Keys.UID, "");
                cookie.Expires = DateTime.Now.AddYears(-3);
                Response.Cookies.Add(cookie);
            }

            //4.0 提醒登录成功，由js去跳转
            // return Json(new { status = 0, msg = "登录成功" });
            return WriteSuccess("登录成功");
        }


        public ActionResult Vcode()
        {
            //FileResult 返回
            byte[] imgbuffer = null;
            //1.0 产生一个随机的字符串
            string vcode = GetVcode(1);

            //2.0 存入到Session中
            Session[Keys.Vcode] = vcode;

            //3.0 画图
            using (Image img = new Bitmap(60, 25))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    g.Clear(Color.White);
                    g.DrawString(vcode, new Font("黑体", 16, FontStyle.Bold), Brushes.Red, 0, 0);
                }

                //将img的流转换成imgbuffer
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imgbuffer = ms.ToArray();
                    //ms.Position = 0; //表示读取指针归零
                    //ms.Read(imgbuffer, 0, (int)ms.Length); 
                }
            }

            return File(imgbuffer, "image/jpeg");
        }

        Random r = new Random();
        private string GetVcode(int count)
        {
            string vcodeString = "23456789abcdefghkmnpqrstuxyzABCDEFGHKMNPQRSTUXYZ";
            int leng = vcodeString.Length;
            string resultString = string.Empty;

            for (int i = 0; i < count; i++)
            {
                resultString += vcodeString[r.Next(leng)];
            }

            return resultString;
        }

        public ViewResult logout()
        {
            //清除Session
            Session.Abandon();
            //清除cookie
            HttpCookie cookie = Request.Cookies[Keys.UID];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie);

            }


            return View("/Areas/admin/Views/Login/Login.cshtml");
        }
        /*      
         * requestform
         * public requestformController(
            IwfRequestFormServices requestformSer,
            IwfProcessServices processSer,
            IwfWorkServices workSer,
            IsysKeyValueServices keyvalSer,
            IwfWorkNodesServices worknodesSer,
            IsysRoleServices roleSer,
            IsysUserInfo_RoleServices userinfoRoleSer
            ) 构造函数，传入相关服务类
                public ActionResult getDetails(int id)  获取当前工作流审批处理对象和查看详情页面类对象
                

         wfprocess
         * 
           public wfprocessController(
            IwfProcessServices pSer,
            IwfRequestFormServices rfSer,
            IwfWorkNodesServices nodeSer,
            IwfWorkServices workSer,
            IsysUserInfo_RoleServices urSer,
            IsysRoleServices roleSer,
            IsysKeyValueServices keyvalSer,
            IwfWorkBranchServices workbranchSer
            )   构造函数，传入相关服务类
         
           public ActionResult checkform(int id)  获取当前工作流审批处理对象和查看详情页面类对象
         * public ActionResult reject(FormCollection f) 处理“拒绝”操作
         * public ActionResult pass(FormCollection f)   处理“通过”操作
         * public ActionResult back(FormCollection f)   处理“驳回”操作
         
         
         wfwork
         *   public wfworkController(IwfWorkServices wSer,
            IwfWorkNodesServices nodeSer,
            IsysKeyValueServices keyvalSer,
            IwfWorkBranchServices workbranchSer
            )   构造函数，传入相关服务类
         *public ActionResult Index()
        public ActionResult getlist(FormCollection f)   获取工作流定义节点
        public ActionResult setNodes(int id)   返回设置节点页面类对象
         *  public ActionResult getNodes(int id)   获取当前工作流的节点列表 
         *  public ActionResult AddNode(int id) 返回添加节点页面类对象
         * public ActionResult AddNode(int id, wfWorkNodes model)   处理添加节点
         * public ActionResult EditNode(int id) 返回编辑节点页面类对象
         * public ActionResult EditNode(int id, wfWorkNodes model)  处理编辑节点
         public ActionResult up(int id) 处理节点上移
          public ActionResult down(int id)   处理节点下移
         public ActionResult delNode(int id)    处理删除节点
         
         public ActionResult setBranch(int id)  弹出设置节点分支页面类对象
         
         public ActionResult getBranchList(int id)  处理ajax请求，返回节点分支列表
         
         
         private void SetKeyValue() 获取节点类型、角色类型列表，并存入下拉菜单绑定项
         private void SetMethods()  获取执行节点逻辑判断方法，并存入下拉菜单绑定项
         
         
         */
    }
}
