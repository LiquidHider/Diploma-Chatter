using AutoMapper;
using Chatter.Domain.BusinessLogic.Extensions;
using Chatter.Domain.BusinessLogic.Interfaces;
using Chatter.Domain.BusinessLogic.Mapping.Configuration;
using Chatter.Domain.BusinessLogic.MessagesContainers;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.DataAccess.Interfaces;
using Chatter.Domain.DataAccess.Models;
using Microsoft.Extensions.Logging;

namespace Chatter.Domain.BusinessLogic.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IChatUserRepository _chatUserRepository;
        private readonly ILogger<ReportService> _logger;
        private readonly IMapper _mapper;

        public ReportService(IReportRepository reportRepository, IChatUserRepository chatUserRepository, ILogger<ReportService> logger)
        {
            _reportRepository = reportRepository;
            _chatUserRepository = chatUserRepository;
            _logger = logger;
            _mapper = new AutoMapperConfguration()
                    .Configure()
                    .CreateMapper();
        }

        public async Task<ValueServiceResult<Report>> CreateReport(CreateReport createReportModel, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<Report>();
            try
            {
                _logger.LogInformation("CreateReport : {@details}", new { Class = nameof(ReportService), Method = nameof(CreateReport) });
                var reportedUser = await _chatUserRepository.GetAsync(createReportModel.ReportedUserID, cancellationToken);

                if (reportedUser == null)
                {
                    _logger.LogInformation("User being reported does not exist. {@Details}", new { createReportModel.ReportedUserID });
                    return result.WithBusinessError(ReportServiceMessagesContainer.UserNotExist);
                }

                var report = new Report()
                {
                    ID = new Guid(),
                    Title = createReportModel.Title,
                    Message = createReportModel.Message,
                    ReportedUserID = createReportModel.ReportedUserID,
                };
                var mappedReport = _mapper.Map<ReportModel>(report);
                await _reportRepository.CreateAsync(mappedReport, cancellationToken);
                _logger.LogInformation("Report was created. {@Details}", new { ReportId = mappedReport.ID, ReportedUserId = mappedReport.ReportedUserID });
                return result.WithValue(report);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex,ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> RemoveReport(Guid id, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<Guid>();

            try
            {
                _logger.LogInformation("RemoveReport : {@details}", new { Class = nameof(ReportService), Method = nameof(RemoveReport) });
                var deletionStatus = await _reportRepository.DeleteAsync(id, cancellationToken);

                if (deletionStatus == Common.Enums.DeletionStatus.NotExisted)
                {
                    _logger.LogInformation("Report with id {@Details} does not exist.", new { ReportId = id });
                    return result.WithBusinessError(ReportServiceMessagesContainer.ReportNotExist);
                }
                _logger.LogInformation("Report deleted. {@Details}", new { ReportId = id });
                return result.WithValue(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<Guid>> SendReport(Report report, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<Guid>();
            try
            {
                var reportedUser = await _chatUserRepository.GetAsync(report.ReportedUserID, cancellationToken);

                if (reportedUser == null)
                {
                    _logger.LogInformation("User being reported does not exist. {@Details}", new { report.ReportedUserID });
                    return result.WithBusinessError(ReportServiceMessagesContainer.UserNotExist);
                }

                _logger.LogInformation("SendReport : {@details}", new { Class = nameof(ReportService), Method = nameof(SendReport) });
                var mappedReport = _mapper.Map<ReportModel>(report);
                await _reportRepository.CreateAsync(mappedReport, cancellationToken);
                _logger.LogInformation("Report sent. {@Details}", new { ReportId = mappedReport.ID });
                return result.WithValue(mappedReport.ID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }
    }
}
