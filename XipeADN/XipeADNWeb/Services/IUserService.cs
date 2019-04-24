using System;
using System.Threading.Tasks;
using XipeADNWeb.Models;

namespace XipeADNWeb.Services
{
    public interface IUserService
    {
        Task<UserModel> AuthenticateAccount(string email, string password);
        Task<Boolean> RegisterAccount(RegisterModel model);
        Task<Boolean> DeactivateAccount(string token, bool hardDelete = false);

        Task<Boolean> EditProfile(UserModel model);
    }
}
