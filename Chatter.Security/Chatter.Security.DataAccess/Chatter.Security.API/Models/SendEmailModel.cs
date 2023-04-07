namespace Chatter.Security.API.Models
{
    public class SendEmailModel
    {
        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public List<string> Attachments { get; } = new List<string>();

        public bool IsBodyHtml { get; set; }
    }
}
