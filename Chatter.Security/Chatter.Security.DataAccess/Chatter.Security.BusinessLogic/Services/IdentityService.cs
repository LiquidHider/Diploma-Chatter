﻿using Chatter.Security.DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using Chatter.Security.Core.Models;
using AutoMapper;
using Chatter.Security.Core.Mapping.Configuration;
using Chatter.Security.DataAccess.Models;
using Chatter.Security.Core.Extensions;
using Chatter.Security.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Chatter.Security.DataAccess.Repositories;

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
                var searchModel = new GetByEmailOrUserTag() 
                {
                    Email = createModel.Email,
                    UserTag = createModel.UserTag,
                };

                var user = await _identityRepository.GetByEmailOrUserTagAsync(searchModel, cancellationToken);

                if (user != null)
                {
                    return result.WithBusinessError("User with this email or usertag already exists.");
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

                return result.WithValue(mappedCreateModel.ID);
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
                _logger.LogInformation("UpdateAsync : {@Details}", new { Class = nameof(IdentityRepository), Method = nameof(UpdateAsync) });
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
                _logger.LogInformation("DeleteAsync : {@Details}", new { Class = nameof(IdentityRepository), Method = nameof(DeleteAsync) });

                var deletionStatus = await _identityRepository.DeleteAsync(id, cancellationToken);

                if (deletionStatus == Common.Enums.DeletionStatus.NotExisted)
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
                var searchModel = new GetByEmailOrUserTag() 
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
                var searchModel = new GetByEmailOrUserTag()
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
