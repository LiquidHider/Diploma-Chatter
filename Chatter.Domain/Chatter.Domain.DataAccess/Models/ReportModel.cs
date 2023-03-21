namespace Chatter.Domain.DataAccess.Models
{
    public class ReportModel
    {
        public Guid ID { get; set; }

        public Guid ReportedUserID { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
    }
}
