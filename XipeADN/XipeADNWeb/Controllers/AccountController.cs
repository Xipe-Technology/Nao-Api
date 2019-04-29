using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XipeADNWeb.Data;
using XipeADNWeb.Entities;
using XipeADNWeb.Models;
using XipeADNWeb.Services;

namespace XipeADNWeb.Controllers
{
    [ /*Authorize, */ Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly XipeADNDbContext _db;

        public AccountController(IUserService userService) =>
            _userService = userService;

        #region Account 
        [AllowAnonymous, HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userService.AuthenticateAccount(model.Email, model.Password);
                    if (user == null)
                        return NotFound("Correo electrónico o contraseña incorrecta.");
                    return Ok(user);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [AllowAnonymous, HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _userService.RegisterAccount(model);
                    return Ok();
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        [AllowAnonymous, HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromQuery]ResetPassword model)
        {
            if (ModelState.IsValid)
            {

                //var user = await UserManager.FindByEmailAsync(model.Email);
                if (true)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Ok();
                }
                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                //string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //code = HttpUtility.UrlEncode(code);
                //var callbackUrl = $"{Request.Host}/login/reset?code={code}&id={user.Id}";
                //var sender = new EmailsService();
                // var ca_correo = "contacto@cruzazul.com";
                //sender.SendSimpleMessage(new Models.Email { Destination = ca_correo, Subject = "", Body = "" });
            }
            // If we got this far, something failed, redisplay form
            return BadRequest();
        }


        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody]UserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _userService.EditProfile(model))
                        return Ok();
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                var writter = new ImageWriter();
                var upload = await writter.UploadImage(file);
                await _userService.EditProfile(new UserModel { ProfilePicUrl = upload });
                return Ok(new { ProfilePictureUrl = upload });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Deactivate([FromQuery]string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                    return BadRequest(ModelState);
                await _userService.DeactivateAccount(token);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        #endregion

        #region Oportunidades

        [HttpPost("CreateOpportunity")]
        public async Task<IActionResult> CreateOpportunity(Opportunity model)
        {
            try
            {
                model.CreationDate = DateTime.Now;
                model.LastUpdate = DateTime.Now;

                //var id = HttpContext.User.Identity.Name;
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);

                if (user != null)
                {
                    user.Naos = user.Naos + 100;

                    _db.Opportunities.Add(model);
                    _db.Entry(user).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status200OK);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        [HttpPost("DeleteOpportunity")]
        public async Task<IActionResult> DeleteOportunity(Opportunity model)
        {
            try
            {
                if (model.Id > 0)
                {
                    var entidad = _db.Opportunities.FirstOrDefault(o => o.Id == model.Id);
                    entidad.IsDeleted = true;
                    entidad.LastUpdate = DateTime.Now;


                    _db.Entry(entidad).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        [HttpPost("GetOpportunities")]
        public async Task<IActionResult> GetOpportunities()
        {
            try
            {
                var entities = await _db.Opportunities.Where(x => !x.IsDeleted).ToListAsync();
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        [HttpPost("EditOpportunity")]
        public async Task<IActionResult> EditOpportunity(Opportunity model)
        {
            try
            {
                if (model.Id > 0)
                {
                    var entidad = _db.Opportunities.FirstOrDefault(o => o.Id == model.Id);

                    entidad.Title = model.Title;
                    entidad.Website = model.Website;
                    entidad.Description = model.Description;
                    entidad.LastUpdate = DateTime.Now;

                    _db.Entry(entidad).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        #endregion
    }
}
