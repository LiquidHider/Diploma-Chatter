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
        private const string userInfoViewPath = "~/Views/Users/UserInfo.cshtml";

        public ContactsController(IChatUserService chatUserService)
        {
            _chatUserService = chatUserService;
        }
        [Route("user")]
        [HttpGet]
        public async Task<IActionResult> OpenUserInfo([FromQuery]Guid id) {
            var cookie = HttpContext.Request.Cookies["User"];
            var currentUser = JsonConvert.DeserializeObject<SecurityUserResponse>(cookie);
            var response = await _chatUserService.GetChatUser(id, currentUser.Token);

            var responseContent = await response.Content.ReadAsStringAsync();
            var contentMapped = JsonConvert.DeserializeObject
                 <ChatUser>(responseContent);

            var viewModel = new UserInfoViewModel() 
            {
                User = contentMapped,
                IsCurrentUser = currentUser.UserId == contentMapped.ID
            };

            return View(userInfoViewPath, viewModel);
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
