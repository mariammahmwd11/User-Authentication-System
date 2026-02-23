using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.User.Models
{
   public class UserProfilePhotoMetaData
    {
        public int ID { get; set; }
        public String? FileName { get; set; }
        public long FileSize { get; set; }
        public String FileType { get; set; }

        public String? imageUrl { get; set; }
        public DateTime UploadedAt  => DateTime.UtcNow;

        //foreign key
        public string UserId { get; set; }
        [JsonIgnore]
        public App_User AppUser { get; set; }

    }
}
