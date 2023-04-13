using AutoMapper;
using Chatter.Domain.API.Helpers;
using Chatter.Domain.API.Models;
using Chatter.Domain.API.Models.ChatUser;
using Chatter.Domain.BusinessLogic.Interfaces;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Models.Parameters;
using Chatter.Domain.BusinessLogic.Models.Update;
using Chatter.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Chatter.Domain.API.Controllers
{
    [Route("user")]
    public class ChatUserController : BaseAPIController
    {
        private readonly IChatUserService _chatUserService;
        private readonly IPrivateChatService _privateChatService;

        public ChatUserController(IChatUserService chatUserService, IPrivateChatService privateChatService, IMapper mapper) : base(mapper)
        {
            _chatUserService = chatUserService;
            _privateChatService = privateChatService;
        }

        [HttpPost]
        [Route("new")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreatedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateNewUser(CreateChatUserRequest requestModel, CancellationToken token) 
        {
            var mappedRequestModel = _mapper.Map<CreateChatUser>(requestModel);
            var result = await _chatUserService.CreateNewUserAsync(mappedRequestModel, token);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(new CreatedResponse(result.Value.ID));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChatUserResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUser(Guid id, CancellationToken token) 
        {
            var result = await _chatUserService.GetUserAsync(id, token);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            var mappedResponse = _mapper.Map<ChatUserResponse>(result.Value);
            return Ok(mappedResponse);
        }

        [HttpPost]
        [Route("contacts")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResponse<ChatUser, ChatUserSort>))]
        public async Task<IActionResult> GetUserContacts(Guid userId, CancellationToken token) 
        {
            var contacts = await _privateChatService.LoadUserContactsAsync(userId, token);
            if (contacts.Value.Count == 0) 
            {
                return Ok(new PaginatedResponse<ChatUser, ChatUserSort>());
            }

            var chatUsersListRequest = new ChatUserListParameters() {
                PageNumber = 1,
                PageSize = contacts.Value.Count,
                SortOrder = SortOrder.Asc,
                SortBy = ChatUserSort.LastName,
                Users = contacts.Value.Select(x => x.Member2ID).ToList()
            };
            var dbResponse = await _chatUserService.ListAsync(chatUsersListRequest,token);

            var mappedResponse = new PaginatedResponse<ChatUser, ChatUserSort>()
            {
                PageNumber = dbResponse.Value.PageNumber,
                PageSize = dbResponse.Value.PageSize,
                SortOrder = dbResponse.Value.SortOrder,
                SortBy = dbResponse.Value.SortBy,
                TotalCount = dbResponse.Value.TotalCount,
                TotalPages = dbResponse.Value.TotalPages,
                Items = dbResponse.Value.Value
            };
            return Ok(mappedResponse);
        }

        [HttpPut]
        [Route("update")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser(UpdateChatUserRequest requestModel, CancellationToken token) 
        {
            var mappedRequestModel = _mapper.Map<UpdateChatUser>(requestModel);
            var result = await _chatUserService.UpdateUserAsync(mappedRequestModel, token);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(new UpdatedResponse(result.Value));
        }


        [HttpPut]
        [Route("block")]
        [Authorize(Roles = UserRoles.Administrator)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> BlockUser(BlockUserRequest requestModel, CancellationToken token) 
        {
            var mappedRequestModel = _mapper.Map<BlockUser>(requestModel);

            var result = await _chatUserService.BlockUserAsync(mappedRequestModel, token);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(new UpdatedResponse(result.Value));
        }

        [HttpPut]
        [Route("unblock")]
        [Authorize(Roles = UserRoles.Administrator)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnblockUser(Guid id, CancellationToken token)
        {

            var result = await _chatUserService.UnblockUserAsync(id, token);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(new UpdatedResponse(result.Value));
        }

        [HttpDelete]
        [Route("delete")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RemovedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken token)
        {
            var result = await _chatUserService.DeleteUserAsync(id, token);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(new RemovedResponse(result.Value));
        }
    }
}
