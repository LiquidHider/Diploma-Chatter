﻿using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.DbOptions;
using Chatter.Domain.DataAccess.Interfaces;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Pagination;
using Chatter.Domain.DataAccess.Models.Parameters;
using Chatter.Domain.DataAccess.Utilities;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Chatter.Domain.DataAccess.Repositories
{
    internal class PhotoRepository : IPhotoRepository
    {
        private readonly ILogger<PhotoRepository> _logger;
        private readonly DatabaseOptions _dbOptions;
        private readonly PhotoSQLQueryHelper _queryHelper;

        public PhotoRepository(ILogger<PhotoRepository> logger, DatabaseOptions dbOptions)
        {
            _logger = logger;
            _dbOptions = dbOptions;
            _queryHelper = new PhotoSQLQueryHelper();
        }

        public async Task CreateAsync(Photo item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID",item.ID);
            parameters.Add("@Url",item.Url);
            parameters.Add("@IsMain", item.IsMain);
            parameters.Add("@UserID", item.UserID);

            var query = PhotoSQLQueryHelper.CreateQuery;

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query,parameters,cancellationToken: cancellationToken);
                var createQuery = await db.ExecuteAsync(command);
            }
        }

        public async Task<DeletionStatus> DeleteAsync(Photo item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID",item.ID);
            int deletedRows = 0;

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(PhotoSQLQueryHelper.DeleteQuery,parameters,cancellationToken: cancellationToken);
                deletedRows = await db.ExecuteAsync(command);
            }

            if (deletedRows == 0) 
            {
                return DeletionStatus.NotExisted;
            }

            return DeletionStatus.Deleted;
        }

        public async Task<PaginatedResult<Photo,PhotoSort>> ListAsync(PhotosListParameters listParameters, CancellationToken cancellationToken)
        {
            if (listParameters == null) 
            {
                throw new ArgumentNullException(nameof(listParameters));
            }

            var parameters = new DynamicParameters();
            parameters.Add("@PageSize", listParameters.PageSize);
            parameters.Add("@PageNumber", listParameters.PageNumber);

            var sortBy = _queryHelper.Where(listParameters.SortBy.ToString());
            var sortOrder = _queryHelper.OrderBy(listParameters.SortOrder.ToString(),listParameters.SortBy.ToString());
            var filterQueryPart = _queryHelper.BuildFilters(listParameters, parameters);

            var query = string.Format(PhotoSQLQueryHelper.ListQuery, sortBy, filterQueryPart, sortOrder);
            query.Concat($"\n {PhotoSQLQueryHelper.CountQuery}");

            using (DbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var queryResult = await db.QueryMultipleAsync(command);
                
                var entities = await queryResult.ReadAsync<Photo>();
                var totalCount = await queryResult.ReadSingleAsync<int>();

                var totalPages = (int)Math.Ceiling(totalCount / (double)listParameters.PageSize);

                var result = new PaginatedResult<Photo, PhotoSort>
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    PageNumber = listParameters.PageNumber,
                    PageSize = listParameters.PageSize,
                    SortOrder = listParameters.SortOrder,
                    SortBy = listParameters.SortBy,
                    Entities = entities.ToList()
                };

                return result;
            }
        }

        public async Task<Photo> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID",id);

            var query = string.Format(PhotoSQLQueryHelper.GetOneQuery, _queryHelper.Where("[ID] = @ID"));

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var result = await db.QuerySingleAsync<Photo>(query);

                return result;
            }
        }
    }
}