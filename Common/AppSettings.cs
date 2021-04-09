using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WelcomeUser.Common
{
    public class AppSettings
    {
        public string ComercialEmail { get; set; }

        public string OAuthConnection { get; set; }
    }
}
