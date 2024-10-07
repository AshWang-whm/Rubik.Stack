using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Rubik.Identity.AuthServer.Models;
using System.Security.Claims;
using System.Web;

namespace Rubik.Identity.AuthServer.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            var query = HttpContext.Request.Query;
            var return_url = $"/login?returnUrl={HttpUtility.UrlEncode(query["returnUrl"])}";
            return View(new LoginInput
            {
                ReturnUrl = return_url
            });
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginInput input)
        {
            // 检查登录

            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name,"ash")
                    },
                    "cookie"
                ));

            await HttpContext.SignInAsync("cookie", principal);

            return Redirect(input.ReturnUrl!);
        }
    }
}
