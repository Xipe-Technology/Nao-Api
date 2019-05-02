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
        private readonly UserManager<User> _userManager;


        public AccountController(IUserService userService, XipeADNDbContext db, UserManager<User> userManager)
        {
            _userService = userService;
            _db = db;
            _userManager = userManager;
        }

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
        public async Task<IActionResult> ForgotPassword([FromBody]ResetPassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                if (user != null)
                {

                    //string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //code = HttpUtility.UrlEncode(code);
                    //var callbackUrl = $"{Request.Host}/login/reset?code={code}&id={user.Id}";
                    //var sender = new EmailsService();
                    //var ca_correo = "contacto@cruzazul.com";
                    //sender.SendSimpleMessage(new Models.Email { Destination = ca_correo, Subject = "", Body = "" });

                    return Ok();
                }
                else
                {
                    return BadRequest();
                }


            }
            return BadRequest();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
                if (user != null)
                {
                    await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
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

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                var writter = new ImageWriter();
                var upload = await writter.UploadImage(file);
                //await _userService.EditProfile(new UserModel { ProfilePicUrl = upload });
                var path = Path.Combine("images", upload);
                //return Ok(new { ProfilePictureUrl = upload });
                return Ok(path);
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
        public async Task<IActionResult> CreateOpportunity([FromBody]Opportunity model)
        {
            try
            {
                model.CreationDate = DateTime.Now;
                model.LastUpdate = DateTime.Now;

                var id = HttpContext.User.Identity.Name;
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
        public async Task<IActionResult> DeleteOpportunity([FromBody]Opportunity model)
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


        [HttpGet("GetOpportunities")]
        public async Task<IActionResult> GetOpportunities([FromQuery]string query)
        {
            try
            {
                if (query != null)
                {
                    var entities = await _db.Opportunities.Include(f => f.KPIs).Where(x => !x.IsDeleted && (x.Title.Contains(query) || x.Description.Contains(query))).ToListAsync();
                    return Ok(entities);
                }
                else
                {
                    var entities = await _db.Opportunities.Include(f => f.KPIs).Where(x => !x.IsDeleted).ToListAsync();
                    return Ok(entities);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("GetMyOpportunities")]
        public async Task<IActionResult> GetMyOpportunities([FromQuery]string UserId)
        {
            try
            {
                var entities = await _db.Opportunities.Include(f => f.KPIs).Where(x => !x.IsDeleted && x.UserId == UserId).ToListAsync();
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        [HttpPost("EditOpportunity")]
        public async Task<IActionResult> EditOpportunity([FromBody]Opportunity model)
        {
            try
            {
                if (model.Id > 0)
                {
                    var entidad = _db.Opportunities.FirstOrDefault(o => o.Id == model.Id);

                    entidad.Title = model.Title;
                    entidad.Website = model.Website;
                    entidad.Picture = model.Picture;
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

        #region Rank

        [HttpGet("GetRanking")]
        public async Task<IActionResult> GetRanking()
        {
            try
            {
                var users = await _db.Users.Where(x => !x.IsDeleted).Select(x =>
                      new {
                          x.Name,
                          x.Naos,
                      }
                ).OrderByDescending(x => Convert.ToInt32(x.Naos)).ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        #endregion

        #region Chats

        [HttpGet("GetChatsByIds")]
        public async Task<IActionResult> GetChatsByIds(string User1, string User2)
        {
            try
            {
                
            }
            catch (Exception)
            {

                throw;
            }
            return Ok();
        }

        #endregion
    }
}
