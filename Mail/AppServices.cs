using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WelcomeUser.Common;

namespace WelcomeUser.Mail
{
    public class AppServices 
    {
        private readonly AppSettings _appSettings;
        public AppServices (IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
    }
}
