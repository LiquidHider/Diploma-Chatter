using System.Net.Mail;

namespace Chatter.Email.Core.Models
{
    public class EmailMessageModel
    {
        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public List<string> Attachments { get; } = new List<string>();

        public bool IsBodyHtml { get; set; }
    }
}
