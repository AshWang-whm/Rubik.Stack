using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Rubik.Identity.AuthServer.Models;
using Rubik.Identity.Share.Entity;
using Rubik.Identity.Share.Extension;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Web;

namespace Rubik.Identity.AuthServer.Controllers
{
    public class AccountController(IFreeSql freeSql) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginInput
            {
                ReturnUrl = HttpContext.Request.Query["returnUrl"]
            });
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginInput input)
        {
            // 检查登录
            var user = await freeSql.Select<TbUser>()
                .Where(a => a.Code == input.UserName)
                .Where(a=>a.IsDelete==false)
                .FirstAsync();
            if (user == null)
            {
                return Json(new LoginResult { Data=1 });
            }

            // check pwd
            if (string.IsNullOrEmpty(input.UserName) || string.IsNullOrEmpty(input.Password))
            {
                return Json(new LoginResult { Data = 1 });
            }

            var pwd = PasswordEncryptExtension.GeneratePasswordHash(input.UserName,input.Password);
            if(pwd!=user.Password)
            {
                return Json(new LoginResult { Data = 2 });
            }

            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.Name,user.Name!),new Claim(ClaimTypes.Sid,user.Code!)]
                , "oidc.cookie"
                ));

            await HttpContext.SignInAsync("oidc.cookie", principal);

            //return Redirect(input.ReturnUrl!);
            return Json(new LoginResult {Code=1});
        }
    }

    public class LoginResult
    {
        /// <summary>
        /// 1：登录成功，其他：登录失败
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// 1:账号错误，2：密码错误
        /// </summary>
        [JsonPropertyName("data")]
        public int Data { get; set; }

        [JsonPropertyName("msg")]
        public string? Msg { get; set; }
    }
}
