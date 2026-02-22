using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_layer.DTO
{
   public class UserProfileDTO
    {
        
        public IFormFile? imageUrl { get; set; }
    }
}
