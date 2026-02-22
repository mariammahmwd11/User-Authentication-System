using Business_layer.DTO;
using Models.User.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_layer.IServices
{
   public interface IAuthService
    {
        Task<AuthModel> Register(RegisterDTO registerDTO);
        Task<AuthModel> Login(LoginDTO loginDto);
        Task<bool> Revokeing(String refreshtocken);
        Task<AuthModel> RefreshTocken(String refreshtocken);




    }
}
