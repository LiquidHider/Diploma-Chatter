namespace Chatter.Domain.API.Models.Reports
{
    public class SendReportRequest
    {
        public Guid ID { get; set; }

        public Guid ReportedUserID { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
    }
}
