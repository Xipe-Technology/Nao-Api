using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using XipeADNWeb.Config;
using XipeADNWeb.Entities;
using XipeADNWeb.Models;

namespace XipeADNWeb.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly User _currentUser;
        private readonly IQueryable<User> _allUsers;
        private readonly IQueryable<User> _activeUsers;
        private readonly UserManager<User> _userManager;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private const int _tokenExpirationDays = 7;

        public UserService(IOptions<AppSettings> appSettings, UserManager<User> userManager
           /*HttpContextAccessor httpAccessor*/)
        {
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _allUsers = userManager.Users;
            _activeUsers = userManager.Users.Where(user => !user.IsDeleted);
            _tokenHandler = new JwtSecurityTokenHandler();
            //_currentUser = userManager.GetUserAsync(httpAccessor.HttpContext.User).Result;
        }

        #region Account
        public async Task<UserModel> AuthenticateAccount(string email, string password)
        {
            // var user = await _activeUsers.FirstOrDefaultAsync(usr => usr.EmailConfirmed && String.Equals(usr.Email, email));
            var user = await _userManager.FindByEmailAsync(email);
            var result = await _userManager.CheckPasswordAsync(user, password);
            if (user != null && result)
            {
                var token = await GenerateTokenAsync(user);
                var jwt = _tokenHandler.WriteToken(token.value);
                return new UserModel
                {
                    Token = jwt,
                    TokenExpiration = token.expirationDate,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = token.userRole,
                    Id = user.Id,
                    CreationDate = user.CreationDate,
                    LastEditDate = user.LastEditDate
                };
            }
            return null;
        }

        public async Task<Boolean> RegisterAccount(RegisterModel model)
        {
            var entity = new User
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreationDate = DateTime.UtcNow,
                LastEditDate = DateTime.UtcNow,
                EmailConfirmed = true
            };
            var res = await _userManager.CreateAsync(entity, model.Password);
            var roleRes = await _userManager.AddToRoleAsync(entity, "Usuario");
            return res.Succeeded && roleRes.Succeeded;
        }


        public async Task<Boolean> DeactivateAccount(string token, bool hardDelete = false)
        {
            // usemos la token luego
            // aqui va sin token
            if (!hardDelete)
            {
                _currentUser.IsDeleted = true;
                var deleted = await _userManager.UpdateAsync(_currentUser);
                return deleted.Succeeded;
            }
            var res = await _userManager.DeleteAsync(_currentUser);
            return res.Succeeded;
        }
        #endregion

        #region Profile
        public async Task<Boolean> EditProfile(UserModel model)
        {
            var entity = await _userManager.FindByEmailAsync(model.Email);
            if (entity == null)
                return false;
            // entity.Email = model.Email;
            entity.FirstName = model.FirstName ?? entity.FirstName;
            entity.LastName = model.LastName ?? entity.LastName;
            entity.LastEditDate = DateTime.UtcNow;
            entity.PhoneNumber = model.Phone ?? entity.PhoneNumber;
            entity.Company = model.Company ?? entity.Company;
            entity.CompanyRole = model.CompanyRole ?? entity.CompanyRole;
            entity.ProfilePicUrl = model.ProfilePictureUrl ?? entity.ProfilePicUrl;
            var res = await _userManager.UpdateAsync(entity);
            return res.Succeeded;
        }
        #endregion

        #region Private
        private async Task<dynamic> GenerateTokenAsync(User user)
        {
            string role = (await _userManager.GetRolesAsync(user))[0];
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                //new Claim(ClaimTypes.Sid, user.SectionId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            // authentication successful so generate jwt token
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(_tokenExpirationDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            dynamic obj = new
            {
                value = _tokenHandler.CreateToken(tokenDescriptor),
                expirationDate = tokenDescriptor.Expires ?? DateTime.UtcNow,
                userRole = role
            };
            return obj;
        }
        #endregion
    }
}
