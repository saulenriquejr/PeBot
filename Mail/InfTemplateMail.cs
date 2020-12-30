using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WelcomeUser.Mail
{
    public class InfTemplateMail
    {
       //[JsonProperty("bodytext")]
       //public Object bodytext { get; set; }

       [JsonProperty("subject")]
       public string subject { get; set; }
       
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("phone")]
        public string Phone { get; set; }

    }
}
