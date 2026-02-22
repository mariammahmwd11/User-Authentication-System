using Business_layer.DTO;
using Business_layer.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.User.JWT;
using Models.User.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business_layer.Services
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<App_User> userManager;
        private readonly JWTHelper jwt;

        public AuthService(UserManager<App_User> userManager, IOptions<JWTHelper> jwt)
        {
            this.userManager = userManager;
            this.jwt = jwt.Value;
        }

        public JwtSecurityToken CreateToken(App_User user)
        {
            var _claims = new List<Claim>();
            _claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            _claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            _claims.Add(new Claim(JwtRegisteredClaimNames.Jti, new Guid().ToString()));
            var symmetrickey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var _signingCredentials = new SigningCredentials(symmetrickey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken SecurityToken = new JwtSecurityToken(
                issuer: jwt.Issuer,
                audience: jwt.Audience,
                claims: _claims,
                expires: DateTime.UtcNow.AddMinutes(jwt.Exbireinminuts),
               signingCredentials: _signingCredentials
                );
            return SecurityToken;
        }

        public RefereshToken GenerteRefreshTOken()
        {
            var randomnumber = new byte[32];
            var genertor = RandomNumberGenerator.Create();
            genertor.GetBytes(randomnumber);
            return new RefereshToken
            {
                token = Convert.ToBase64String(randomnumber),
                ExbireOn = DateTime.UtcNow.AddDays(30),
                CreatedOn = DateTime.UtcNow,

            };

        }

        public async Task<AuthModel> RefreshTocken(String refreshtocken)
        {
            var model = new AuthModel();
            var user = await userManager.Users.SingleOrDefaultAsync(b => b.refereshTokens.Any(m => m.token == refreshtocken));
            if (user == null)
            {
                model.Message = "Token is invalid";
                return model;

            }
            var token = user.refereshTokens.Single(b => b.token == refreshtocken);
            if (token.IActive == false)
            {
                model.Message = "Token is Not Active";
                return model;
            }
            token.RevokedOn = DateTime.UtcNow;
            var new_Refresh_Token = GenerteRefreshTOken();
            user.refereshTokens?.Add(new_Refresh_Token);
            await userManager.UpdateAsync(user);
            var securiyToken = CreateToken(user);
            model.Message = "Created Successfully";
            model.token = new JwtSecurityTokenHandler().WriteToken(securiyToken);
            model.ExbireOn = DateTime.UtcNow.AddMinutes(jwt.Exbireinminuts);
            model.RefereshToken = new_Refresh_Token.token;
            model.ReferhTokenExbireOn = new_Refresh_Token.ExbireOn;

            return model;
        }

        public async Task<bool> Revokeing(String refreshtocken)
        {
            var model = new AuthModel();
            var user = await userManager.Users.SingleOrDefaultAsync(b => b.refereshTokens.Any(m => m.token == refreshtocken));
            if (user == null)
            {
                return false;

            }
            var token = user.refereshTokens.Single(b => b.token == refreshtocken);
            if (token.IActive == false)
            {
                return false;

            }
            token.RevokedOn = DateTime.UtcNow;
            await userManager.UpdateAsync(user);
            return true;

        }

        public async Task<AuthModel> Login(LoginDTO loginDto)
        {
            var model = new AuthModel();
            var user = await userManager.FindByNameAsync(loginDto.UserName);
            if(user!=null)
            {
                var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
                if(result!=null)
                {
                    var securityTocken = CreateToken(user);
                    if(user.refereshTokens.Any(b=>b.IActive))
                    {
                        var refreshtocken = user.refereshTokens.FirstOrDefault(b => b.IActive);
                        model.RefereshToken = refreshtocken.token;
                        model.ReferhTokenExbireOn = refreshtocken.ExbireOn;
                    }
                    var new_refreshtocken = GenerteRefreshTOken();
                    user.refereshTokens?.Add(new_refreshtocken);
                    await userManager.UpdateAsync(user);
                    model.UserName = user.UserName;
                    model.Message = "Login Successfully";
                    model.token = new JwtSecurityTokenHandler().WriteToken(securityTocken);
                    model.ExbireOn = DateTime.Now.AddMinutes(jwt.Exbireinminuts);
                    model.RefereshToken = new_refreshtocken.token;
                    model.ReferhTokenExbireOn = new_refreshtocken.ExbireOn;
                    return model;
                }
                model.Message = "invalid UserName or PassWord";

            }
            model.Message = "invalid UserName or PassWord";
            return model;
        }

        public async Task<AuthModel> Register(RegisterDTO registerDTO)
        { var model = new AuthModel();
            var user = await userManager.FindByNameAsync(registerDTO.UserName);
            if(user!=null)
            {
                 model.Message = "there is an existing username";
            }
            var userfromdb = new App_User
            {
                UserName = registerDTO.UserName,
               Email=registerDTO.Email
            };
            var result = await userManager.CreateAsync(userfromdb, registerDTO.Password);
            if(result==null)
            {
                var errors = string.Empty;
                foreach(var error in result.Errors)
                {
                    errors += $"{error.Description},";
                    model.Message = errors;
                }
            }
            var securitytocken = CreateToken(userfromdb);
            var RefreshTocken = GenerteRefreshTOken();
            userfromdb.refereshTokens?.Add(RefreshTocken);
            await userManager.UpdateAsync(userfromdb);
            model.UserName = userfromdb.UserName;
            model.Message = "Registered Successfully";
            model.token = new JwtSecurityTokenHandler().WriteToken(securitytocken);
            model.ExbireOn = DateTime.UtcNow.AddMinutes(jwt.Exbireinminuts);
            model.RefereshToken = RefreshTocken.token;
            model.ReferhTokenExbireOn = RefreshTocken.ExbireOn;
            
            return model;
        }
    }
}
