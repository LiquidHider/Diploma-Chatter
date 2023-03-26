using Chatter.Domain.BusinessLogic.Models.Abstract;

namespace Chatter.Domain.BusinessLogic.Models
{
    public class Report
    {
        public Guid ID { get; set; }

        public ChatUser ReportedUser { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime SentUtc { get; set; }
    }
}
