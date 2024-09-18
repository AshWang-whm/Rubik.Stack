using Microsoft.AspNetCore.Mvc;
using Rubik.Identity.AuthServer.Models;

namespace Rubik.Identity.AuthServer.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginInput input)
        {

            return View();
        }
    }
}
