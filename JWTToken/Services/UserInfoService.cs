using JWTToken.Database;
using JWTToken.Helpers;
using JWTToken.IServices;
using JWTToken.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTToken.Services
{
    public class UserInfoService : IUserInfoService
    {
        private List<UserInfo> _users = new List<UserInfo>
        {
            new UserInfo
            {
                UserInfoId = Guid.NewGuid(),
                FullName = "Varsik",
                UserName = "Yan",
                Password = "1323"
            }
        };

        private readonly AppSettings _appSettings;

        public UserInfoService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public UserInfo Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.UserName == username && x.Password == password);
            if (user == null)
                return null;
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name,user.UserInfoId.ToString())}),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDesc);
            user.Token = tokenHandler.WriteToken(token);
            return user;
        }

        public IEnumerable<UserInfo> GetAll()
        {
            return _users;
        }

        public bool Register(UserInfo userInfo)
        {
            if (ModelsValid(userInfo))
            {
                using (var  db = new DatabaseContext())
                {
                    db.Users.Add(userInfo);
                }
            };

            return true;
        }

        public bool ModelsValid(UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(userInfo.EmailId) && string.IsNullOrEmpty(userInfo.Password))
            {
                return false;
            }

            if (userInfo.Password != userInfo.ConfirmPassword)
            {
                return false;
            }
            return true;
        }
    }
}