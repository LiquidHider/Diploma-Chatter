using AutoMapper;
using Chatter.Domain.API.Helpers;
using Chatter.Domain.API.Models;
using Chatter.Domain.API.Models.Reports;
using Chatter.Domain.BusinessLogic.Interfaces;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Chatter.Domain.API.Controllers
{

    [Route("report")]
    public class ReportController : BaseAPIController
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService, IMapper mapper) : base(mapper)
        {
            _reportService = reportService;
        }

        [HttpPost]
        [Route("send")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreatedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateReportAsync(CreateReportRequest requestModel, CancellationToken token) 
        {
            var mappedRequest = _mapper.Map<CreateReport>(requestModel);

            var result = await _reportService.CreateReportAsync(mappedRequest, token);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(new CreatedResponse(result.Value.ID));
        }

        [HttpPost]
        [Route("list")]
        [Authorize(Roles = UserRoles.Administrator)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResponse<ReportResponse,ReportSort>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> ReportsListAsync(ReportsListRequest requestModel, CancellationToken token)
        {
            var mappedRequest = new ReportsListParameters()
            {
                PageNumber = requestModel.PageNumber,
                PageSize = requestModel.PageSize,
                SortOrder = requestModel.SortOrder,
                SortBy = requestModel.SortBy,
                ReportedUsersIDs = requestModel.ReportedUsersIDs
            };

            var result = await _reportService.GetReportsListAsync(mappedRequest, token);
            var mappedResponse = new PaginatedResponse<ReportResponse, ReportSort>()
            {
                PageNumber = result.Value.PageNumber,
                PageSize = result.Value.PageSize,
                SortOrder = result.Value.SortOrder,
                SortBy = result.Value.SortBy,
                TotalCount = result.Value.TotalCount,
                TotalPages = result.Value.TotalPages,
                Items = result.Value.Value.Select(x => _mapper.Map<ReportResponse>(x)).ToList()
            };
            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }
            return Ok(mappedResponse); 
        }


        [HttpDelete]
        [Route("delete")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RemovedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveReportAsync(Guid id, CancellationToken token)
        {

            var result = await _reportService.RemoveReportAsync(id, token);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(new RemovedResponse(result.Value));
        }

    }
}
