using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WelcomeUser.Mail
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
