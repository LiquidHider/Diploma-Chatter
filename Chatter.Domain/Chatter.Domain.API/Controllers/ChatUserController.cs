using AutoMapper;
using Chatter.Domain.API.Helpers;
using Chatter.Domain.API.Models;
using Chatter.Domain.API.Models.ChatUser;
using Chatter.Domain.BusinessLogic.Interfaces;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Models.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Chatter.Domain.API.Controllers
{
    [Route("user")]
    public class ChatUserController : BaseAPIController
    {
        private readonly IChatUserService _chatUserService;

        public ChatUserController(IChatUserService chatUserService, IMapper mapper) : base(mapper)
        {
            _chatUserService = chatUserService;
        }

        [HttpPost]
        [Route("new")]
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
