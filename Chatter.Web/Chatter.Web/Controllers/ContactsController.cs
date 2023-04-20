using Chatter.Web.Enums;
using Chatter.Web.Interfaces;
using Chatter.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Chatter.Web.Controllers
{
    [Route("users")]
    public class ContactsController : Controller
    {
        private readonly IChatUserService _chatUserService;
        private const string usersListViewPath = "~/Views/Users/Users.cshtml";

        public ContactsController(IChatUserService chatUserService)
        {
            _chatUserService = chatUserService;
        }

        [Route("all")]
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber, int pageSize)
        {
            var cookie = HttpContext.Request.Cookies["User"];
            var currentUser = JsonConvert.DeserializeObject<SecurityUserResponse>(cookie);
            var getAllUsersResponse = await _chatUserService.GetAllUsersList(pageNumber, pageSize, currentUser.Token);

            var getAllUsersContent = await getAllUsersResponse.Content.ReadAsStringAsync();
            var getAllUsersContentMapped = JsonConvert.DeserializeObject
                 <PaginatedResponse<ChatUser, ChatUserSort>>(getAllUsersContent);
            var getCurrentChatUserResponse = await _chatUserService.GetChatUser(currentUser.UserId, currentUser.Token);
           
            var getCurrentChatUserContent = await getCurrentChatUserResponse.Content.ReadAsStringAsync();
            var getCurrentChatUserMapped = JsonConvert.DeserializeObject
                 <ChatUser>(getCurrentChatUserContent);

            var viewModel = new ContactsViewModel() {
                PaginatedContactsList = getAllUsersContentMapped,
                CurrentChatUser = getCurrentChatUserMapped
            };
            return View(usersListViewPath, viewModel);
        }
    }
}
