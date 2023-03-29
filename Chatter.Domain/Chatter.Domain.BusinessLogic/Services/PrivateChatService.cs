using AutoMapper;
using Chatter.Domain.BusinessLogic.Extensions;
using Chatter.Domain.BusinessLogic.Interfaces;
using Chatter.Domain.BusinessLogic.Mapping.Configuration;
using Chatter.Domain.BusinessLogic.MessagesContainers;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Abstract;
using Chatter.Domain.BusinessLogic.Models.ChatMessages;
using Chatter.Domain.BusinessLogic.Models.Chats;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Models.Update;
using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Interfaces;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;
using Microsoft.Extensions.Logging;


namespace Chatter.Domain.BusinessLogic.Services
{
    public class PrivateChatService : IPrivateChatService
    {
        private readonly IChatUserRepository _chatUserRepository;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly ILogger<PrivateChatService> _logger;
        private readonly IMapper _mapper;

        public PrivateChatService(IChatUserRepository chatUserRepository, IChatMessageRepository chatMessageRepository, ILogger<PrivateChatService> logger)
        {
            _chatUserRepository = chatUserRepository;
            _chatMessageRepository = chatMessageRepository;
            _logger = logger;
            _mapper = new AutoMapperConfguration()
                  .Configure()
                  .CreateMapper();
        }

        public async Task<ValueServiceResult<PrivateChat>> CreateNewChat(User member1, User member2, CancellationToken token)
        {
            var result = new ValueServiceResult<PrivateChat>();
            try
            {
                _logger.LogInformation("CreateNewChat : {@Details}", new { Class = nameof(PrivateChatService), Method = nameof(CreateNewChat) });

                var chatMember1 = await _chatUserRepository.GetAsync(member1.ID,token);
                var chatMember2 = await _chatUserRepository.GetAsync(member2.ID,token);

                if (chatMember1 is null || chatMember2 is null) 
                {
                    _logger.LogInformation("One or both chat members does not exist. Taken from db: {@Details}", new { Member1 = chatMember1, Member2 = chatMember2 });
                    return result.WithBusinessError(PrivateChatServiceMessagesContainer.OneOrBothMembersDoesNotExist);
                }

                var chat = new PrivateChat() 
                {
                    Member1 = member1,
                    Member2 = member2
                };

                return result.WithValue(chat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> DeleteMessage(Guid messageId, CancellationToken token)
        {
            var result = new ValueServiceResult<Guid>();
            try
            {
                _logger.LogInformation("DeleteMessage : {@Details}", new { Class = nameof(PrivateChatService), Method = nameof(DeleteMessage) });

                var deletionStatus = await _chatMessageRepository.DeleteAsync(messageId, token);

                if (deletionStatus == DeletionStatus.NotExisted) 
                {
                    _logger.LogInformation("Message does not exist. {@Details}", new { MessageID = messageId });
                    return result.WithBusinessError(ReportServiceMessagesContainer.UserNotExist);
                }

                return result.WithValue(messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> EditMessage(UpdateMessage updateModel, CancellationToken token)
        {
            var result = new ValueServiceResult<Guid>();
            try 
            {
                _logger.LogInformation("EditMessage : {@Details}", new { Class = nameof(PrivateChatService), Method = nameof(EditMessage) });
                updateModel.IsEdited = true;
                var mappedUpdateModel = _mapper.Map<UpdateChatMessageModel>(updateModel);
                var isSuccessful = await _chatMessageRepository.UpdateAsync(mappedUpdateModel, token);

                if (!isSuccessful)
                {
                    _logger.LogInformation("Message does not exist. {@Details}", new { MessageID = updateModel.ID });
                    return result.WithBusinessError(ReportServiceMessagesContainer.UserNotExist);
                }

                return result.WithValue(updateModel.ID);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
}

        public async Task<ValueServiceResult<List<PrivateChatMessage>>> LoadChat(PrivateChat chat, CancellationToken token)
        {
            var result = new ValueServiceResult<List<PrivateChatMessage>>();
            try
            {
                _logger.LogInformation("LoadChat : {@Details}", new { Class = nameof(PrivateChatService), Method = nameof(LoadChat) });

                var listParameters = new ChatMessageListParameters() 
                {
                    SortOrder = SortOrder.Asc,
                    SortBy = ChatMessageSort.Sent,
                    RecipientIsGroup = false,
                    Sender = chat.Member1.ID,
                    Recipient = chat.Member2.ID
                };

                var messages = await _chatMessageRepository.ListAsync(listParameters, token);

                if (messages.Count == 0) 
                {
                    _logger.LogInformation("Chat is empty. {@Details}", new { Member1 = chat.Member1.ID, Member2 = chat.Member2.ID });
                    return result.WithValue(new List<PrivateChatMessage>());
                }

                var mappedMessages = messages.Select(x => new PrivateChatMessage() 
                {
                    ID = x.ID,
                    IsRead = x.IsRead,
                    IsEdited = x.IsEdited,
                    Body = x.Body,
                    Sent = x.Sent,
                    Sender = chat.Member1.ID == x.Sender ? chat.Member1 : chat.Member2,
                    Recipient = chat.Member1.ID == x.RecipientUser ? chat.Member1 : chat.Member2
                }).ToList();

                return result.WithValue(mappedMessages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public Task<ValueServiceResult<List<PrivateChat>>> LoadContacts(Guid userId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<ValueServiceResult<Guid>> MarkMessageAsRead(Guid messageId, CancellationToken token)
        {
            var result = new ValueServiceResult<Guid>();
            try
            {
                _logger.LogInformation("MarkMessageRead : {@Details}", new { Class = nameof(PrivateChatService), Method = nameof(MarkMessageAsRead) });

                var updateModel = new UpdateMessage() 
                {
                    ID = messageId,
                    IsRead = true
                };
                var mappedUpdateModel = _mapper.Map<UpdateChatMessageModel>(updateModel);
                var isSuccessful = await _chatMessageRepository.UpdateAsync(mappedUpdateModel, token);

                if (!isSuccessful)
                {
                    _logger.LogInformation("Message does not exist. {@Details}", new { MessageID = messageId });
                    return result.WithBusinessError(ReportServiceMessagesContainer.UserNotExist);
                }

                return result.WithValue(messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> SendMessageAsync(CreateChatMessage createModel, CancellationToken token)
        {
            var result = new ValueServiceResult<Guid>();
            try
            {
                var member1 = await _chatUserRepository.GetAsync(createModel.SenderID, token);
                var member2 = await _chatUserRepository.GetAsync(createModel.RecipientID, token);

                if (member1 is null || member2 is null) 
                {
                    _logger.LogInformation("One or both declared members does not exist. Taken from db: {@Details}", new { Member1 = member1, Member2 = member2 });
                    return result.WithBusinessError(PrivateChatServiceMessagesContainer.OneOrBothMembersDoesNotExist);
                }
                var formatedSendTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ff");
                var messageModel = new ChatMessageModel() 
                {
                    ID = Guid.NewGuid(),
                    Body = createModel.Body,
                    IsEdited = false,
                    Sent = DateTime.Parse(formatedSendTime),
                    IsRead = false,
                    Sender = createModel.SenderID,
                    RecipientUser = createModel.RecipientID,
                };

                await _chatMessageRepository.CreateAsync(messageModel, token);

                return result.WithValue(messageModel.ID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }
    }
}
