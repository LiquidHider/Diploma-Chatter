using Chatter.Security.DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using Chatter.Security.Core.Models;
using AutoMapper;
using Chatter.Security.Core.Mapping.Configuration;
using Chatter.Security.DataAccess.Models;
using Chatter.Security.Core.Extensions;
using Chatter.Security.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Chatter.Security.Common.Enums;

namespace Chatter.Security.Core.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IdentityService> _logger;
        private readonly IIdentityRepository _identityRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IHMACEncryptor _hmacEncryptor;
        private readonly IMapper _mapper;

        public IdentityService(IIdentityRepository identityRepository, IConfiguration configuration, IUserRoleRepository userRoleRepository,
            IHMACEncryptor hmacEncryptor, ILogger<IdentityService> logger)
        {
            _identityRepository = identityRepository;
            _userRoleRepository = userRoleRepository;
            _configuration = configuration;
            _logger = logger;
            _hmacEncryptor = hmacEncryptor;
            _mapper = new AutoMapperConfguration()
              .Configure()
              .CreateMapper();
        }

        public async Task<ValueServiceResult<Guid>> CreateAsync(CreateIdentity createModel, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<Guid>();
            try
            {
                _logger.LogInformation("CreateAsync : {@Details}", new { Class = nameof(IdentityService), Method = nameof(CreateAsync) });
                var searchModel = new EmailOrUserTagSearchModel() 
                {
                    Email = createModel.Email,
                    UserTag = createModel.UserTag,
                };

                var user = await _identityRepository.GetByEmailOrUserTagAsync(searchModel, cancellationToken);

                if (user != null)
                {
                    _logger.LogInformation("Identity with specified email or usertag already exists. {@Details}", new { UserTag = createModel.UserTag, Email = createModel.Email  });
                    return result.WithBusinessError("Identity with specified email or usertag already exists.");
                }

                string passwordKey = _hmacEncryptor.CreateRandomPasswordKey(_configuration);
                string passwordHash = _hmacEncryptor.EncryptPassword(createModel.Password, Encoding.UTF8.GetBytes(passwordKey));

                var mappedCreateModel = new CreateIdentityModel()
                {
                    ID = Guid.NewGuid(),
                    Email = createModel.Email,
                    UserTag = createModel.UserTag,
                    PasswordHash = passwordHash,
                    PasswordKey = passwordKey,
                    UserID = createModel.UserID
                };
            
                await _identityRepository.CreateAsync(mappedCreateModel, cancellationToken);

                foreach (var role in createModel.Roles) 
                {
                    await _userRoleRepository.AddRoleToUserAsync(mappedCreateModel.ID, role, cancellationToken);
                }

                return result.WithValue(mappedCreateModel.ID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> AddRoleToIdentityAsync(Guid identityId, UserRole userRole, CancellationToken cancellationToken) 
        {
            var result = new ValueServiceResult<Guid>();

            try 
            {
                _logger.LogInformation("AddRoleToIdentityAsync : {@Details}", new { Class = nameof(IdentityService), Method = nameof(AddRoleToIdentityAsync) });

                var identity = await _identityRepository.GetAsync(identityId, cancellationToken);

                if (identity == null) 
                {
                    _logger.LogInformation("Identity does not exist. {@Details}", new { IdentityID = identityId });
                    return result.WithBusinessError("Identity does not exist.");
                }

                var dbUserRoleId = await _userRoleRepository.GetRoleIdAsync(identityId, userRole, cancellationToken);

                if (dbUserRoleId != Guid.Empty) 
                {
                    _logger.LogInformation("Specified identity role already exists. {@Details}", new { IdentityID = identityId, UserRole = userRole.ToString() });
                    return result.WithBusinessError("Specified identity role already exists.");
                }

                var roleId = await _userRoleRepository.AddRoleToUserAsync(identityId, userRole, cancellationToken);

                return result.WithValue(roleId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> RemoveRoleIdentityAsync(Guid identityId, UserRole userRole, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<Guid>();

            try
            {
                _logger.LogInformation("RemoveRoleFromIdentityAsync : {@Details}", new { Class = nameof(IdentityService), Method = nameof(RemoveRoleIdentityAsync)});
                
                var identity = await _identityRepository.GetAsync(identityId, cancellationToken);

                if (identity == null)
                {
                    _logger.LogInformation("Identity does not exist. {@Details}", new { IdentityID = identityId });
                    return result.WithBusinessError("Identity does not exist.");
                }

                var userRoleId = await _userRoleRepository.GetRoleIdAsync(identityId, userRole, cancellationToken);
                var deletionStatus = await _userRoleRepository.DeleteUserRoleAsync(identityId, userRole, cancellationToken);

                if (deletionStatus == DeletionStatus.NotExisted)
                {
                    _logger.LogInformation("Specified role for identity  does not exist. {@Details}", new { IdentityID = identityId, UserRole = userRole.ToString() });
                    return result.WithBusinessError("Specified role for identity does not exist.");
                }

                return result.WithValue(userRoleId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> UpdateAsync(UpdateIdentity updateModel, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<Guid>();
            try
            { 
                _logger.LogInformation("UpdateAsync : {@Details}", new { Class = nameof(IdentityService), Method = nameof(UpdateAsync) });
                var mappedModel = _mapper.Map<UpdateIdentityModel>(updateModel);

                var isModified = await _identityRepository.UpdateAsync(mappedModel, cancellationToken);

                if (!isModified)
                {
                    _logger.LogInformation("Identity with specific ID does not exist. {@Details}", new { IdentityID = updateModel.ID });
                    return result.WithBusinessError("Identity does not exist.");
                }

                return result.WithValue(updateModel.ID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<Guid>();
            try 
            {
                _logger.LogInformation("DeleteAsync : {@Details}", new { Class = nameof(IdentityService), Method = nameof(DeleteAsync) });

                var deletionStatus = await _identityRepository.DeleteAsync(id, cancellationToken);

                if (deletionStatus == DeletionStatus.NotExisted)
                {
                    _logger.LogInformation("Identity with specific ID does not exist. {@Details}", new { IdentityID = id });
                    return result.WithBusinessError("Identity does not exist.");
                }

                return result.WithValue(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Identity>> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<Identity>();
            try
            {
                _logger.LogInformation("FindByIdAsync : {@Details}", new { Class = nameof(IdentityService), Method = nameof(FindByIdAsync) });
                var identity = await _identityRepository.GetAsync(id, cancellationToken);

                if (identity == null) 
                {
                    _logger.LogInformation("Identity with specific ID does not exist. {@Details}", new { IdentityID = id });
                    return result.WithBusinessError("Identity does not exist.");
                }
                var mappedIdentity = _mapper.Map<Identity>(identity);

                return result.WithValue(mappedIdentity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Identity>> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<Identity>();
            try
            {
                _logger.LogInformation("FindByEmailAsync : {@Details}", new { Class = nameof(IdentityService), Method = nameof(FindByEmailAsync) });
                var searchModel = new EmailOrUserTagSearchModel() 
                {
                    Email = email
                };

                var identity = await _identityRepository.GetByEmailOrUserTagAsync(searchModel, cancellationToken);

                if (identity == null)
                {
                    _logger.LogInformation("Identity with specific email does not exist. {@Details}", new { Email = searchModel.Email });
                    return result.WithBusinessError("Identity does not exist.");
                }

                var mappedIdentity = _mapper.Map<Identity>(identity);

                return result.WithValue(mappedIdentity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Identity>> FindByUserTagAsync(string userTag, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<Identity>();
            try
            {
                _logger.LogInformation("FindByUserTagAsync : {@Details}", new { Class = nameof(IdentityService), Method = nameof(FindByUserTagAsync) });
                var searchModel = new EmailOrUserTagSearchModel()
                {
                    UserTag = userTag
                };

                var identity = await _identityRepository.GetByEmailOrUserTagAsync(searchModel, cancellationToken);

                if (identity == null)
                {
                    _logger.LogInformation("Identity with specific user tag does not exist. {@Details}", new { UserTag = searchModel.UserTag });
                    return result.WithBusinessError("Identity does not exist.");
                }

                var mappedIdentity = _mapper.Map<Identity>(identity);

                return result.WithValue(mappedIdentity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<IList<string>>> GetRolesAsync(Guid identityId, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<IList<string>>();
            try
            {
                _logger.LogInformation("GetRolesAsync : {@Details}", new { Class = nameof(IdentityService), Method = nameof(GetRolesAsync) });
                var identity = await _identityRepository.GetAsync(identityId, cancellationToken);

                if (identity == null)
                {
                    _logger.LogInformation("Identity with specific Id does not exist. {@Details}", new { IdentityId = identityId });
                    return result.WithBusinessError("Identity does not exist.");
                }

                var roles = await _userRoleRepository.GetUserRolesAsync(identityId, cancellationToken);
                IList<string> list = roles.Select(x => x.ToString()).ToList();
                return result.WithValue(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }
    }
}
