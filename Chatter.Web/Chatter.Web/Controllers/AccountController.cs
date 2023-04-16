using Chatter.Web.Interfaces;
using Chatter.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;

namespace Chatter.Web.Controllers
{
    public class AccountController : Controller
    {
        private const string signInPagePath = "~/Views/Account/SignIn.cshtml";
        private const string signUpPagePath = "~/Views/Account/SignUp.cshtml";

        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
             _accountService = accountService;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> SignInPage()
        {
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
            var responeContent = await response.Content.ReadAsStringAsync();
            Response.Cookies.Append("User", responeContent);
            
            return RedirectToAction("", "Chat");
        }

        [Route("signUp")]
        [HttpGet]
        public async Task<IActionResult> SignUpPage() 
        {
            return View(signUpPagePath);
        }
    }
}
