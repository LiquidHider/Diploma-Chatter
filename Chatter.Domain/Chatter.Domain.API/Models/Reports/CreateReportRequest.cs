namespace Chatter.Domain.API.Models.Reports
{
    public class CreateReportRequest
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public Guid ReportedUserID { get; set; }

    }
}
