using Chatter.Web.Enums;
using Chatter.Web.Interfaces;
using Chatter.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Chatter.Web.Controllers
{
    [Route("Chat")]
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IAccountService _accountService;
        private readonly IChatUserService _chatUserService;

        public ChatController(ILogger<ChatController> logger, IAccountService accountService, IChatUserService chatUserService)
        {
            _logger = logger;
            _accountService = accountService;
            _chatUserService = chatUserService;
           
        }

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var dfg = HttpContext.Request.Cookies["User"];
            var currentChatUser = JsonConvert.DeserializeObject<SecurityUserResponse>(dfg);
            var getChatUserResponse = await _chatUserService.GetChatUser(currentChatUser.UserId, currentChatUser.Token);
            var getChatUserContent = await getChatUserResponse.Content.ReadAsStringAsync();

            var userContactsResponse = await _chatUserService.GetChatUserContacts(currentChatUser.UserId, currentChatUser.Token);
            var userContactsContent = await userContactsResponse.Content.ReadAsStringAsync();
            var userContactsMapped = JsonConvert.DeserializeObject
                 <PaginatedResponse<ChatUser, ChatUserSort>>(userContactsContent).Items;

            var chatWindowVM = new ChatWindowViewModel()
            {
                currentUser = JsonConvert.DeserializeObject<ChatUser>(getChatUserContent),
                contacts = userContactsMapped
            };

            return View(chatWindowVM);
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