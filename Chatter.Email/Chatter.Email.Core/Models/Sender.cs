namespace Chatter.Email.Core.Models
{
    public class Sender
    {
        public string SmtpAddress { get; set; }

        public int PortNumber { get; set; }

        public bool EnableSSL { get; set; }

        public string FromAddress { get; set; }

        public string FromPassword { get; set; }

    }
}
