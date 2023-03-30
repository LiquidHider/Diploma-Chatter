using AutoMapper;
using Chatter.Domain.BusinessLogic.Extensions;
using Chatter.Domain.BusinessLogic.Interfaces;
using Chatter.Domain.BusinessLogic.Mapping.Configuration;
using Chatter.Domain.BusinessLogic.MessagesContainers;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Models.Update;
using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Interfaces;
using Chatter.Domain.DataAccess.Models;
using Microsoft.Extensions.Logging;

namespace Chatter.Domain.BusinessLogic.Services
{
    public class ChatUserService : IChatUserService
    {
        private readonly IChatUserRepository _chatUserRepository;
        private readonly ILogger<ChatUserService> _logger;
        private readonly IMapper _mapper;

        public ChatUserService(IChatUserRepository chatUserRepository, ILogger<ChatUserService> logger)
        {
            _chatUserRepository = chatUserRepository;
            _logger = logger;
            _mapper = new AutoMapperConfguration()
                  .Configure()
                  .CreateMapper();
        }

        public async Task<ValueServiceResult<ChatUser>> CreateNewUserAsync(CreateChatUser createModel, CancellationToken token)
        {
            return await Task.Run(() =>
            {
                var result = new ValueServiceResult<ChatUser>();
                try
                {
                    _logger.LogInformation("CreateNewUserAsync : {@Details}", new { Class = nameof(ChatUserService), Method = nameof(CreateNewUserAsync) });

                    if (createModel is null) 
                    {
                        throw new ArgumentNullException(nameof(createModel));
                    }
                    
                    var formatedJoinedTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ff");
                    var chatUser = new ChatUser()
                    {
                        ID = Guid.NewGuid(),
                        LastName = createModel.LastName,
                        FirstName = createModel.FirstName,
                        Patronymic = createModel.Patronymic,
                        UniversityName = createModel.UniversityName,
                        UniversityFaculty = createModel.UniversityFaculty,
                        JoinedUtc = DateTime.Parse(formatedJoinedTime),
                    };

                    return result.WithValue(chatUser);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return result.WithException(ex.Message);
                }
            });
        }

        public async Task<ValueServiceResult<Guid>> DeleteUserAsync(Guid id, CancellationToken token)
        {
            var result = new ValueServiceResult<Guid>();
            try
            {
                _logger.LogInformation("DeleteUserAsync : {@Details}", new { Class = nameof(ChatUserService), Method = nameof(DeleteUserAsync) });

                var deletionStatus = await _chatUserRepository.DeleteAsync(id, token);

                if (deletionStatus == DeletionStatus.NotExisted)
                {
                    _logger.LogInformation("User does not exist. {@Details}", new { UserD = id });
                    return result.WithBusinessError(ChatUserServiceMessagesContainer.UserNotExist);
                }

                return result.WithValue(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<ChatUser>> GetUserAsync(Guid id, CancellationToken token)
        {
            var result = new ValueServiceResult<ChatUser>();
            try
            {
                _logger.LogInformation("UpdateUserAsync : {@Details}", new { Class = nameof(ChatUserService), Method = nameof(UpdateUserAsync) });
                var user = await _chatUserRepository.GetAsync(id, token);

                if (user is null)
                {
                    _logger.LogInformation("User does not exist. {@Details}", new { UserID = id });
                    return result.WithBusinessError(ChatUserServiceMessagesContainer.UserNotExist);
                }

                var mappedUser = _mapper.Map<ChatUser>(user);
                var contacts = await _chatUserRepository.GetUserContactsAsync(id, token);

                if (contacts is not null) 
                {
                    mappedUser.Contacts = contacts.Select(x => new Models.Chats.PrivateChat()
                    {
                        Member1ID = user.ID,
                        Member2ID = x
                    }).ToList();
                }
                else 
                {
                    mappedUser.Contacts = new();
                }
                
                return result.WithValue(mappedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> UpdateUserAsync(UpdateChatUser updateModel, CancellationToken token)
        {
            var result = new ValueServiceResult<Guid>();
            try
            {
                _logger.LogInformation("UpdateUserAsync : {@Details}", new { Class = nameof(ChatUserService), Method = nameof(UpdateUserAsync) });
                var userToUpdate = await _chatUserRepository.GetAsync(updateModel.UserID, token);

                if (userToUpdate is null) 
                {
                    _logger.LogInformation("User does not exist. {@Details}", new { UserID = updateModel.UserID });
                    return result.WithBusinessError(ChatUserServiceMessagesContainer.UserNotExist);
                }

                var mappedUpdateModel = _mapper.Map<UpdateChatUserModel>(updateModel);

                var isModified = await _chatUserRepository.UpdateAsync(mappedUpdateModel, token);

                if (!isModified)
                {
                    return result.WithDataError(ChatUserServiceMessagesContainer.BadRequestError);
                }

                return result.WithValue(updateModel.UserID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> BlockUserAsync(BlockUser blockModel, CancellationToken token)
        {
            var result = new ValueServiceResult<Guid>();
            try
            {
                _logger.LogInformation("BlockUserAsync : {@Details}", new { Class = nameof(ChatUserService), Method = nameof(UpdateUserAsync) });
                var userToBlock = await _chatUserRepository.GetAsync(blockModel.UserID, token);

                if (userToBlock is null)
                {
                    _logger.LogInformation("User does not exist. {@Details}", new { UserID = blockModel.UserID });
                    return result.WithBusinessError(ChatUserServiceMessagesContainer.UserNotExist);
                }

                if (userToBlock.IsBlocked) 
                {
                    _logger.LogInformation("User is already blocked. {@Details}", new { UserID = blockModel.UserID,
                        BlockedUntil = userToBlock.BlockedUntil });
                    return result.WithBusinessError(ChatUserServiceMessagesContainer.UserAlreadyBlocked);
                }
                var mappedBlockModel = new UpdateChatUserModel() 
                {
                    ID = blockModel.UserID,
                    IsBlocked = blockModel.IsBlocked,
                    BlockedUntil = blockModel.BlockedUntilUtc,
                };
                var isModified = await _chatUserRepository.UpdateAsync(mappedBlockModel, token);

                if (!isModified)
                {
                    _logger.LogInformation("Bad request error occured.");
                    return result.WithDataError(ChatUserServiceMessagesContainer.BadRequestError);
                }

                return result.WithValue(blockModel.UserID);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> UnblockUserAsync(Guid id, CancellationToken token)
        {
            var result = new ValueServiceResult<Guid>();
            try
            {
                _logger.LogInformation("UnblockUserAsync : {@Details}", new { Class = nameof(ChatUserService), Method = nameof(UpdateUserAsync) });
                var userToUnblock = await _chatUserRepository.GetAsync(id, token);

                if (userToUnblock is null)
                {
                    _logger.LogInformation("User does not exist. {@Details}", new { UserID = id });
                    return result.WithBusinessError(ChatUserServiceMessagesContainer.UserNotExist);
                }

                if (!userToUnblock.IsBlocked)
                {
                    _logger.LogInformation("User is not blocked. {@Details}", new{ UserID = id });
                    return result.WithBusinessError(ChatUserServiceMessagesContainer.UserIsNotBlocked);
                }

                var mappedBlockModel = new UpdateChatUserModel()
                {
                    ID = id,
                    IsBlocked = false,
                    BlockedUntil = null,
                };

                var isModified = await _chatUserRepository.UpdateAsync(mappedBlockModel, token);

                if (!isModified)
                {
                    _logger.LogInformation("Bad request error occured.");
                    return result.WithDataError(ChatUserServiceMessagesContainer.BadRequestError);
                }

                return result.WithValue(id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }
    }
}
