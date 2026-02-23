using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.User.Models
{
  public  class App_User:IdentityUser
    {
        public List<RefereshToken> refereshTokens { get; set; } = new();
        public string? imageUrl { get; set; }
        public List<UserProfilePhotoMetaData>? photos { get; set; } = new();
    }
}
