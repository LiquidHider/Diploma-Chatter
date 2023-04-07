using Chatter.Email.Common.ServiceResults;
using Chatter.Email.Core.Interfaces;
using Chatter.Email.Common.Extensions;
using Chatter.Email.Core.Models;
using System.Net.Mail;
using System.Net;
using System.Text.Json;

namespace Chatter.Email.Core.Service
{
    public class SmtpEmailService : IEmailService
    {
        private readonly Sender _sender;

        public SmtpEmailService()
        {
            _sender = GetSender();
        }

        public ServiceResult SendEmail(EmailMessageModel model)
        {
            var result = new ServiceResult();
           
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(_sender.FromAddress);
                if (model.To == string.Empty) 
                {
                    return result.WithDataError("Recipient address is empty.");
                }
                mail.To.Add(model.To);
                mail.Subject = model.Subject;
                mail.Body = model.Body;
                mail.IsBodyHtml = model.IsBodyHtml;
                if(model.Attachments.Count > 0) 
                {
                    foreach (var attachment in model.Attachments)
                    {
                        mail.Attachments.Add(new Attachment(attachment));
                    }
                }

                using (SmtpClient smtp = new SmtpClient(_sender.SmtpAddress, _sender.PortNumber))
                {
                    smtp.Credentials = new NetworkCredential(_sender.FromAddress, _sender.FromPassword);
                    smtp.EnableSsl = _sender.EnableSSL;
                    smtp.Send(mail);
                }
            }

            return result;
        }
        public Sender GetSender()
        {
            const string SenderFilename = @"..\..\..\..\Chatter.Email.Core\Sender.json";

            var json = File.ReadAllText(SenderFilename);
            return JsonSerializer.Deserialize<Sender>(json);
        }
    }
}
