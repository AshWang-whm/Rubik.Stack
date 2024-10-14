$(function () {
    //滑块验证方式 弹出式popup，嵌入式embed，悬浮hover
    var mode = 'popup';

    //获取输入参数
    function getInput() {
        var input = {
            userName: $.trim($("#userName").val()),
            password: $.trim($("#password").val()),
            rememberLogin: !!$("#rememberLogin:checked").val(),
            returnUrl: $("#returnUrl").val(),
            __RequestVerificationToken: $("input[name='__RequestVerificationToken']:first").val()
        }
        return input;
    }

    var timmerId = null;
    //显示消息
    function showMsg(msg) {
        if (msg) {
            $(".my-alert:first").show().text(msg);
            if (timmerId) {
                clearTimeout(timmerId);
            }
            timmerId = window.setTimeout(function () {
                $(".my-alert:first").hide().text('');
            }, 3000);
        }
    }

    //验证登录信息
    function validate() {
        var $userName = $("#userName");
        if ($.trim($userName.val()) === '') {
            $userName.focus();
            $("#lblUserName").show();
            return false;
        }

        var $password = $("#password");
        if ($.trim($password.val()) === '') {
            $password.focus();
            $("#lblPassword").show();
            return false;
        }
        return true;
    }
    //用户名检查
    $("#userName").blur(function () {
        if ($.trim($(this).val()) === '') {
            $("#lblUserName").show();
        } else {
            $("#lblUserName").hide();
        }
    });
    //密码检查
    $("#password").blur(function () {
        if ($.trim($(this).val()) === '') {
            $("#lblPassword").show();
        } else {
            $("#lblPassword").hide();
        }
    });

    var width = $('.form-group:first').width() + 'px';

    function login() {
        var isValid = validate();
        if (!isValid) {
            return false;
        }

        var $me = $("#btnLogin");
        $me.prop('disabled', true).addClass('is-disabled').text('登录中...');
        var input = getInput();
        
        $.ajax({
            type: "post",
            url: '/account/login',
            data: input,
            cache: false,
            success: function (res) {
                if (!res) {
                    $me.prop('disabled', false).removeClass('is-disabled').text('重新登录');
                    return;
                }
                if (res.code === 1) {
                    var returnUrl = $.trim($("#returnUrl").val());
                    console.log(returnUrl);
                    if (returnUrl) {
                        window.location.href = returnUrl;
                    }
                } else {
                    $me.prop('disabled', false).removeClass('is-disabled').text('重新登录');
                    var msg = res.msg;
                    if (res.data === 1) {
                        msg = '您的账号输入不正确，请重新输入';
                        $("#userName").focus();
                    } else if (res.data === 2) {
                        msg = '您的密码输入不正确，请重新输入';
                        $("#password").focus();
                    }
                    showMsg(msg);
                }
            },
            fail: function (err) {
                $me.prop('disabled', false).removeClass('is-disabled').text('重新登录');
                showMsg('服务器异常');
            }
        })
    }

    //登录
    $("#btnLogin").click(function () {
        login();
        return false;
    });

    var userDefaults = {
        plat: {
            userName: 'user',
            password: '111111'
        },
        tenant: {
            userName: '18988889999',
            password: '111111'
        }
    };
    $(".my-radio-group .my-radio-button__inner").click(function () {
        $(".my-radio-group .my-radio-button__inner.active").removeClass('active');
        $(this).addClass("active");
        var userType = $(this).data("value");
        var user = userDefaults[userType];
        if (user) {
            $("#userName").val(user.userName);
            $("#password").val(user.password);
        }
    });
});
