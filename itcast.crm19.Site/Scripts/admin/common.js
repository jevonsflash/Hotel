var helper = {};
helper.localWindow = null;
helper.openPanel = this.openPanel;
helper.returnStatus = this.returnStatus;
helper.ajaxBegin = this.ajaxBegin;
helper.ajaxComplete = this.ajaxComplete;
helper.closePanel = this.closePanel;
helper.ajaxSuccess = this.ajaxSuccess;
helper.getFunction = this.getFunction;
function openPanel(url, title, width, height) {
    //设置默认值
    var w = 450;
    var h = 450;
    //覆盖默认值
    if (width) {
        w = width;
    }
    if (height) {
        h = height;
    }
    openTip();

    helper.localWindow = $.ligerDialog.open({
        url: url,
        title: title,
        width: w,
        height: h,
        onLoaded: closeTip
    })
}

function returnStatus(ajaxobj, callBackFun) {
    //ajaxobj对象的格式:{status:0/1/2,msg:}
    if (ajaxobj.status == 1) {
        $.ligerDialog.error(ajaxobj.msg,"错误提示");
        return;
    }
    //未登录处理
    if (ajaxobj.status == 2) {
        $.ligerDialog.error(ajaxobj.msg, "未登录", function () { window.top.location = "/admin/login/Index"; });
        setTimeout(function () { window.top.location = "/admin/login/Index"; }, 2000);
        return;
    }
    //没权限处理
    if (ajaxobj.status == 3) {
        $.ligerDialog.error(ajaxobj.msg, "没权限执行此操作");
        return;
    }

    //由于正常逻辑处理根据业务不同而不同
    callBackFun(); //回调方法
}
function closePanel() {
    helper.localWindow.close();
}
function ajaxBegin() {
    openTip();
}
function ajaxSuccess(ajaxobj) {

    helper.returnStatus(ajaxobj, function () {
        $.ligerDialog.success(ajaxobj.msg, "成功提示");

    })
}

function ajaxComplete() {
    closeTip();
}
function openTip() {
    var w = $("body").width() / 2;
    var h = $("body").height() / 2;
    $("#loadpage").css("left", w).css("top", h).show();
}
function closeTip() {
    $("#loadpage").hide();
}
//将权限按钮封装
function getFunction(callback) {
    var mid = window.top.tab.getSelectedTabItemID();
    $.post("/Admin/GetFunction/GenerateFunction/" + mid, null, function (ajaxObj) {
        for (var i = 0; i < ajaxObj.length; i++) {
            if (ajaxObj[i].click) {
                //通过eval()将字符串变成一个函数的指针
                ajaxObj[i].click = eval(ajaxObj[i].click);
            }
        }
        callback(ajaxObj);
    }, "json")
}