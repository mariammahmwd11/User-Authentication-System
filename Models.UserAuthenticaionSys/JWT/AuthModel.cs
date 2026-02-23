using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.User.JWT
{
   public class AuthModel
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime ExbireOn { get; set; }
        public string token { get; set; }
        [JsonIgnore]
        public string RefereshToken { get; set; }
        public DateTime ReferhTokenExbireOn { get; set; }

    }
}
