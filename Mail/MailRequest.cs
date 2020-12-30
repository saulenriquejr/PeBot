using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WelcomeUser.Mail
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string  Description { get; set; }
        public string Phone { get; set; }

        public List<IFormFile> Attachments { get; set; }
        public IList<Attachment> BotAttachments { get; set; }
    }
}
