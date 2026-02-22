using Business_layer.DTO;
using Business_layer.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.User.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace User_Authentication_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<App_User> user;

        public AccountController(IAuthService authService,IWebHostEnvironment webHostEnvironment,UserManager<App_User> user)
        {
            this.authService = authService;
            this.webHostEnvironment = webHostEnvironment;
            this.user = user;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if(ModelState.IsValid)
            {
                var result = await authService.Register(registerDTO);
                if(result.RefereshToken!=null)
                {
                    SetRefereshTockenInCookie(result.RefereshToken, result.ReferhTokenExbireOn);
                }
                return Ok(result);

            }
            return BadRequest();

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await authService.Login(loginDTO);
                if (result.RefereshToken != null)
                {
                    SetRefereshTockenInCookie(result.RefereshToken, result.ReferhTokenExbireOn);
                }
                return Ok(result);

            }
            return BadRequest();
        }
        private void SetRefereshTockenInCookie(string refershtocken,DateTime ExbirOn)
        {
            var cookieoption = new CookieOptions
            {
                Expires = ExbirOn.ToLocalTime(),
                IsEssential=true,
                HttpOnly=true,
                SameSite=SameSiteMode.None,
                Secure=true
            };
            Response.Cookies.Append("RefereshTocken", refershtocken, cookieoption);
           
        }



        //profile 
        [HttpPut ("UpdateProfilephoto")]
        public async Task<IActionResult> UpdateProfilePhoto([FromForm]UserProfileDTO dTO)
        {  
            //check for Size
            var maxsize = 2 * 1024 * 1024;
            if(dTO.imageUrl.Length==0||dTO.imageUrl==null)
            {
                return BadRequest("There is no Photo");

            }
            if(dTO.imageUrl.Length>maxsize)
            {
                return BadRequest("The size should not be larger than 2Mb");
            }

            //check for extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension= Path.GetExtension(dTO.imageUrl.FileName).ToLower();
            if(!allowedExtensions.Contains(extension))
            {
                return BadRequest("This Extension is not allowed");
            }




            //جبت الباث بتاع ال wwroot
            var rootbath = webHostEnvironment.WebRootPath;
           //اسم الفايل 
           
            var fileName = Guid.NewGuid().ToString() +extension;
          
            // الباث الل بيتخزن فيه 
            var productPath = Path.Combine(rootbath, @"images/User");

            if(!Directory.Exists(productPath))
            {
                Directory.CreateDirectory(productPath);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingUser =await user.FindByIdAsync(userId);
            if(existingUser.imageUrl!=null)
            {
                var oldPath = Path.Combine(rootbath, existingUser.imageUrl.TrimStart('/'));
                System.IO.File.Delete(oldPath);
            }
            var metadata = new UserProfilePhotoMetaData();
            using (var filestream=new FileStream(Path.Combine(rootbath, fileName), FileMode.Create))
            {
                dTO.imageUrl.CopyTo(filestream);

                metadata.FileName = fileName;
                    metadata.FileType = Path.GetExtension(fileName);
                metadata.imageUrl = Path.Combine(rootbath, fileName);
                     metadata.UserId = userId;
                    metadata.FileSize = dTO.imageUrl.Length;

                existingUser.photos.Add(metadata);
            }
            
            existingUser.imageUrl = @"/images/User/" + fileName;

            await user.UpdateAsync(existingUser);
            return Ok(metadata);
           
            
        }
        [HttpDelete("DeleteProfilephoto")]
        public async Task<IActionResult> DeleteProfilePhoto()
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingUser =await user.FindByIdAsync(userid);
            var rootpath = webHostEnvironment.WebRootPath;
            if (existingUser.imageUrl!=null)
            {
                var oldpath = Path.Combine(rootpath, existingUser.imageUrl.TrimStart('/'));
                System.IO.File.Delete(oldpath);
            }
            return NoContent();
        }
        
    }
}
