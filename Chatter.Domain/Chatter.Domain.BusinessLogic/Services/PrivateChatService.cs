using AutoMapper;
using Chatter.Domain.BusinessLogic.Extensions;
using Chatter.Domain.BusinessLogic.Interfaces;
using Chatter.Domain.BusinessLogic.Mapping.Configuration;
using Chatter.Domain.BusinessLogic.MessagesContainers;
using Chatter.Domain.BusinessLogic.Models;
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

        public async Task<ValueServiceResult<PrivateChat>> CreateChatAsync(Guid member1ID, Guid member2ID, CancellationToken token)
        {
            var result = new ValueServiceResult<PrivateChat>();
            try
            {
                _logger.LogInformation("CreateNewChat : {@Details}", new { Class = nameof(PrivateChatService), Method = nameof(CreateChatAsync) });

                var chatMember1 = await _chatUserRepository.GetAsync(member1ID,token);
                var chatMember2 = await _chatUserRepository.GetAsync(member2ID,token);

                if (chatMember1 is null || chatMember2 is null) 
                {
                    _logger.LogInformation("One or both chat members does not exist. Taken from db: {@Details}", new { Member1 = member1ID, Member2 = member2ID });
                    return result.WithBusinessError(PrivateChatServiceMessagesContainer.OneOrBothMembersDoesNotExist);
                }

                var chat = new PrivateChat() 
                {
                    Member1ID = member1ID,
                    Member2ID = member2ID
                };

                return result.WithValue(chat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> DeleteMessageAsync(Guid messageId, CancellationToken token)
        {
            var result = new ValueServiceResult<Guid>();
            try
            {
                _logger.LogInformation("DeleteMessage : {@Details}", new { Class = nameof(PrivateChatService), Method = nameof(DeleteMessageAsync) });

                var deletionStatus = await _chatMessageRepository.DeleteAsync(messageId, token);

                if (deletionStatus == DeletionStatus.NotExisted) 
                {
                    _logger.LogInformation("Message does not exist. {@Details}", new { MessageID = messageId });
                    return result.WithBusinessError(PrivateChatServiceMessagesContainer.MessageDoesNotExist);
                }

                return result.WithValue(messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> EditMessageAsync(UpdateMessage updateModel, CancellationToken token)
        {
            var result = new ValueServiceResult<Guid>();
            try 
            {
                _logger.LogInformation("EditMessage : {@Details}", new { Class = nameof(PrivateChatService), Method = nameof(EditMessageAsync) });
                updateModel.IsEdited = true;
                var mappedUpdateModel = _mapper.Map<UpdateChatMessageModel>(updateModel);
                var isSuccessful = await _chatMessageRepository.UpdateAsync(mappedUpdateModel, token);

                if (!isSuccessful)
                {
                    _logger.LogInformation("Message does not exist. {@Details}", new { MessageID = updateModel.ID });
                    return result.WithBusinessError(PrivateChatServiceMessagesContainer.MessageDoesNotExist);
                }

                return result.WithValue(updateModel.ID);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
}

        public async Task<ValueServiceResult<List<PrivateChatMessage>>> LoadChatAsync(PrivateChat chat, CancellationToken token)
        {
            var result = new ValueServiceResult<List<PrivateChatMessage>>();
            try
            {
                _logger.LogInformation("LoadChat : {@Details}", new { Class = nameof(PrivateChatService), Method = nameof(LoadChatAsync) });

                var listParameters = new ChatMessageListParameters() 
                {
                    SortOrder = SortOrder.Asc,
                    SortBy = ChatMessageSort.Sent,
                    RecipientIsGroup = false,
                    Sender = chat.Member1ID,
                    Recipient = chat.Member2ID
                };

                var chatMember1 = await _chatUserRepository.GetAsync(chat.Member1ID, token);
                var chatMember2 = await _chatUserRepository.GetAsync(chat.Member2ID, token);

                if (chatMember1 is null || chatMember2 is null)
                {
                    _logger.LogInformation("One or both chat members does not exist. Taken from db: {@Details}", new { Member1 = chat.Member1ID, Member2 = chat.Member2ID });
                    return result.WithBusinessError(PrivateChatServiceMessagesContainer.OneOrBothMembersDoesNotExist);
                }

                var messages = await _chatMessageRepository.ListAsync(listParameters, token);

                if (messages.Count == 0) 
                {
                    _logger.LogInformation("Chat is empty. {@Details}", new { Member1 = chat.Member1ID, Member2 = chat.Member2ID });
                    return result.WithValue(new List<PrivateChatMessage>());
                }

                var mappedMessages = messages.Select(x => new PrivateChatMessage() 
                {
                    ID = x.ID,
                    IsRead = x.IsRead,
                    IsEdited = x.IsEdited,
                    Body = x.Body,
                    Sent = x.Sent,
                    Sender = chat.Member1ID == x.Sender ? chat.Member1ID : chat.Member2ID,
                    RecipientID = chat.Member1ID == x.RecipientUser ? chat.Member1ID : chat.Member2ID
                }).ToList();

                return result.WithValue(mappedMessages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<List<PrivateChat>>> LoadUserContactsAsync(Guid userId, CancellationToken token)
        {
            var result = new ValueServiceResult<List<PrivateChat>>();

            try
            {
                _logger.LogInformation("LoadUserContacts : {@Details}", new { Class = nameof(PrivateChatService), Method = nameof(LoadUserContactsAsync) });

                var user = await _chatUserRepository.GetAsync(userId, token);
                if (user is null)
                {
                    _logger.LogInformation("User does not exist. {@Details}", new { UserID = userId });
                    return result.WithBusinessError(PrivateChatServiceMessagesContainer.UserDoesNotExist);
                }
                var contacts = await _chatUserRepository.GetUserContactsAsync(userId, token);

                var chats = contacts.Select(x => new PrivateChat()
                {
                    Member1ID = userId,
                    Member2ID = x 
                }).ToList();

                return result.WithValue(chats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> MarkMessageAsReadAsync(Guid messageId, CancellationToken token)
        {
            var result = new ValueServiceResult<Guid>();
            try
            {
                _logger.LogInformation("MarkMessageRead : {@Details}", new { Class = nameof(PrivateChatService), Method = nameof(MarkMessageAsReadAsync) });

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
                    return result.WithBusinessError(PrivateChatServiceMessagesContainer.MessageDoesNotExist);
                }

                return result.WithValue(messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<PrivateChatMessage>> SendMessageAsync(CreateChatMessage createModel, CancellationToken token)
        {
            var result = new ValueServiceResult<PrivateChatMessage>();
            try
            {
                var member1 = await _chatUserRepository.GetAsync(createModel.SenderID, token);
                var member2 = await _chatUserRepository.GetAsync(createModel.RecipientID, token);

                if (member1 is null || member2 is null) 
                {
                    _logger.LogInformation("One or both declared members does not exist. Taken from db: {@Details}", new { Sender = createModel.SenderID, Recipient = createModel.RecipientID });
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
                var response = await _chatMessageRepository.GetAsync(messageModel.ID, token);
                return result.WithValue(_mapper.Map<PrivateChatMessage>(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }
    }
}
