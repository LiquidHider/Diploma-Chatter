using AutoMapper;
using Chatter.Domain.API.Models;
using Chatter.Domain.API.Models.PrivateChat;
using Chatter.Domain.BusinessLogic.Interfaces;
using Chatter.Domain.BusinessLogic.Models.Chats;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Models.Update;
using Microsoft.AspNetCore.Mvc;

namespace Chatter.Domain.API.Controllers
{
    
    [Route("chat")]
    public class PrivateChatController : BaseAPIController
    {
        private readonly IPrivateChatService _privateChatService;

        public PrivateChatController(IMapper mapper, IPrivateChatService privateChatService) : base(mapper)
        {
            _privateChatService = privateChatService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChatMessageResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> OpenChat(OpenPrivateChatRequest requestModel, CancellationToken cancellationToken)
        {
            var mappedRequestModel = _mapper.Map<PrivateChat>(requestModel);
            var result = await _privateChatService.LoadChatAsync(mappedRequestModel, cancellationToken);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(result.Value.Select(x => _mapper.Map<ChatMessageResponse>(x)).ToList());
        }

        [HttpPost]
        [Route("send-message")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreatedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SendPrivateMessage(CreateChatMessageRequest requestModel, CancellationToken cancellationToken)
        {
            var mappedModel = _mapper.Map<CreateChatMessage>(requestModel);

            var result = await _privateChatService.SendMessageAsync(mappedModel, cancellationToken);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(new CreatedResponse(result.Value));
        }

        [HttpPut]
        [Route("mark-as-read")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkMessageAsRead(Guid id, CancellationToken cancellationToken)
        {
            var result = await _privateChatService.MarkMessageAsReadAsync(id, cancellationToken);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(new UpdatedResponse(result.Value));
        }

        [HttpPut]
        [Route("edit-message")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EditMessage(UpdateChatMessageRequest requestModel, CancellationToken cancellationToken)
        {
            var mappedRequestModel = _mapper.Map<UpdateMessage>(requestModel);

            var result = await _privateChatService.EditMessageAsync(mappedRequestModel, cancellationToken);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(new UpdatedResponse(result.Value));
        }

        [HttpDelete]
        [Route("delete-message")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RemovedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteMessage(Guid id, CancellationToken cancellationToken)
        {
            var result = await _privateChatService.DeleteMessageAsync(id, cancellationToken);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(new RemovedResponse(result.Value));
        }
    }
}
