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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Job = user.Job,
                    ProfilePicUrl = user.ProfilePicUrl,
                    Location = user.Location,
                    BannerPicUrl = user.BannerPicUrl,
                    Company = user.Company,
                    Id = user.Id,
                    CreationDate = user.CreationDate,
                    LastEditDate = user.LastUpdate,
                    Matches = user.Matches,
                    Opportunities = user.Opportunities,
                    Naos = user.Naos,
                    Twitter = user.Twitter,
                    LinkedIn = user.LinkedIn,
                    About = user.About,
                    CountryCode = user.CountryCode
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
                Name = model.Name,
                CreationDate = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var res = await _userManager.CreateAsync(entity, model.Password);
            var roleRes = await _userManager.AddToRoleAsync(entity, "Usuario");
            return res.Succeeded && roleRes.Succeeded;
        }


        public async Task<UserModel> ForgotPassword(string email)
        {
            // var user = await _activeUsers.(usr => usr.EmailConfirmed && String.Equals(usr.Email, email));
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return null;
            }
            return null;
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
            entity.Company = model.Company ?? entity.Company;
            entity.Job = model.Job ?? entity.Job;
            entity.CountryCode = model.CountryCode ?? entity.CountryCode;
            entity.PhoneNumber = model.PhoneNumber ?? entity.PhoneNumber;
            entity.About = model.About ?? entity.About;
            entity.LinkedIn = model.LinkedIn ?? entity.LinkedIn;
            entity.Twitter = model.Twitter ?? entity.Twitter;
            entity.LastUpdate = DateTime.UtcNow;
            // entity.Email = model.Email;
            // entity.Name = model.Name ?? entity.Name;
            // entity.CompanyRole = model.CompanyRole ?? entity.CompanyRole;
            entity.ProfilePicUrl = model.ProfilePicUrl ?? entity.ProfilePicUrl;
            var res = await _userManager.UpdateAsync(entity);
            return res.Succeeded;
        }
        #endregion

        #region Private
        private async Task<dynamic> GenerateTokenAsync(User user)
        {
            //string role = (await _userManager.GetRolesAsync(user))[0];
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                //new Claim(ClaimTypes.Sid, user.SectionId.ToString()),
                //new Claim(ClaimTypes.Role, role)
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
                //userRole = role
            };
            return obj;
        }
        #endregion
    }
}
