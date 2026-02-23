using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.User.Models
{
    [Owned]
   public class RefereshToken
    {
        public string token { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExbireOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsExbire => DateTime.UtcNow >= ExbireOn;
        public bool IActive => RevokedOn == null && !IsExbire;

    }
}
