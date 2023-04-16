using Chatter.Web.Interfaces;
using Chatter.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chatter.Web.Controllers
{
    public class AccountController : Controller
    {
        private const string signInPagePath = "~/Views/Account/SignIn.cshtml";

        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
             _accountService = accountService;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> SignInPage()
        {
            var currentUser = Request.Cookies["User"];

            if (currentUser != null)
            {
                return RedirectToAction("", "Chat");
            }

            return View(signInPagePath);
        }


        [Route("signIn")]
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInModel signInModel)
        {
            var response = await _accountService.SignIn(signInModel);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || !ModelState.IsValid)
            {
                signInModel.WrongCreds = true;
                return View(signInPagePath, signInModel);
            }
            var userJson = await response.Content.ReadAsStringAsync();

            Response.Cookies.Append("User", userJson);

            return RedirectToAction("", "Chat");
        }

    }
}
