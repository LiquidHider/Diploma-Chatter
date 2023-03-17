namespace Chatter.Domain.DataAccess.Models
{
    internal class Report
    {
        public Guid ID { get; set; }

        public Guid ReportedUserId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
    }
}
