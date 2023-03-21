namespace Chatter.Domain.DataAccess.Models
{
    public class Report
    {
        public Guid ID { get; set; }

        public Guid ReportedUserID { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
    }
}
