using Microsoft.Extensions.Options;
using MimeKit;
//using SendGrid;
//using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using static WelcomeUser.Mail.InfTemplateMail;

namespace WelcomeUser.Mail
{
    public class MailService : IMailService
    {
        public readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            // var email = new SendGridMessage();
            // email.SetFrom(_mailSettings.Mail);
            //var recipients = new List<EmailAddress> { new EmailAddress(mailRequest.ToEmail, "Pevaar Client") };
            // email.AddTos(recipients);
            // email.SetSubject(mailRequest.Subject);
            var builder = new BodyBuilder();
            //if (mailRequest.Attachments != null)
            //{
            //    byte[] fileBytes;
            //    foreach (var file in mailRequest.Attachments)
            //    {
            //        if (file.Length > 0)
            //        {
            //            using (var ms = new MemoryStream())
            //            {
            //                file.CopyTo(ms);
            //                fileBytes = ms.ToArray();
            //            }
            //            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
            //        }
            //    }
            //}

            
            builder.HtmlBody = mailRequest.Body;
            // //email.AddContent(MimeType.Html, mailRequest.Body);
            // email.SetTemplateId("d-f39627877d8042f3b3ec9d14143b1bf9");
            // email.SetTemplateData(new InfTemplateMail { subject=mailRequest.Subject, Name=mailRequest.Name, Phone=mailRequest.Phone, Description=mailRequest.Description, Email=mailRequest.Email});

            // //using var smtp = new SmtpClient();
            // //smtp.Connect(_mailSettings.Host, _mailSettings.Port);
            // //smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            // //await smtp.SendAsync(email);
            // //smtp.Disconnect(true);
            // var apiKey = _mailSettings.SENDGRID_API_KEY;
            //var client = new SendGridClient(apiKey);

            var recipients = new List<MailAddress> { new MailAddress(mailRequest.ToEmail, "Pevaar Client") };
            foreach (MailAddress mail in recipients)
            {
                MailMessage message = new MailMessage(new MailAddress(_mailSettings.Mail), mail);                
                message.Subject = mailRequest.Subject;
                message.Body = "Elizabeth, Long time no talk. Would you be up for lunch in Soho on Monday? I'm paying.;";
                if (mailRequest.Subject == "Cotización" && mailRequest.BotAttachments != null) { mailRequest.BotAttachments.Clear(); }
            else if (mailRequest.BotAttachments != null && mailRequest.Subject != "Cotización")
            {
                foreach (var file in mailRequest.BotAttachments)
                {
                    using (var webClient = new WebClient())
                    {
                        byte[] fileBytes;
                        var filestream = webClient.OpenRead(file.ContentUrl);

                        using (var ms = new MemoryStream())
                        {
                            filestream.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                            //Byte[] bytes = File.ReadAllBytes(filestream.);
                            Attachment data = new Attachment(filestream, "Hoja de vida" , file.ContentType);//Convert.ToBase64String(fileBytes), file.ContentType);
                        message.Attachments.Add(data);
                    }
                    
                }
            }
                

                var client = new SmtpClient("smtp.mailtrap.io", 2525)
                {
                    Credentials = new NetworkCredential("090e5144e5a987", "9e8017cd05ddc5"),
                    EnableSsl = true
                };

                var response = client.SendMailAsync(message);
                
                //var response = await client.SendEmailAsync(email);
            }
        }
    }
}
