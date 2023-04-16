using Chatter.Web.Interfaces;
using Chatter.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Chatter.Web.Controllers
{
    [Route("Chat")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountService _accountService;

        public HomeController(ILogger<HomeController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var currentUser = Request.Cookies["User"];

            if (currentUser == null)
            {
                return RedirectToAction("");
            }
            return View();
        }

        [Route("Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}