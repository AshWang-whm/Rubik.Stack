using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using System.Diagnostics;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            var id_token = await HttpContext.GetTokenAsync("id_token");
            System.Diagnostics.Debug.WriteLine($"id_token:{id_token}");
            var access_token = await HttpContext.GetTokenAsync("access_token");
            System.Diagnostics.Debug.WriteLine($"token:{access_token}");
            foreach (var item in HttpContext.User.Claims)
            {
                System.Diagnostics.Debug.WriteLine($"{item.Type} : {item.Value}");
            }

            // token access jwt test
            var client = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:5166")
            };
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {access_token}");
            var data = await client.GetStringAsync("weatherforecast");

            System.Diagnostics.Debug.WriteLine(data);


            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
