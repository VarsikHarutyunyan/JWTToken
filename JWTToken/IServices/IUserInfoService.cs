using JWTToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTToken.IServices
{
    public interface IUserInfoService
    {
        UserInfo Authenticate(string username,string password);
        bool Register(UserInfo userInfo);
    }
}
