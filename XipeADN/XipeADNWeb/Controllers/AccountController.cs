using System;
using System.Collections.Generic;
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
                    var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Invalid Credentials, please verify and try again.");
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            return BadRequest(ModelState);
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
                    var entities = await _db.Opportunities.Include(f => f.KPIs).Include(p => p.User).Where(x => !x.IsDeleted && (x.Title.Contains(query) || x.Description.Contains(query))).ToListAsync();
                    return Ok(entities);
                }
                else
                {
                    var entities = await _db.Opportunities.Include(f => f.KPIs).Include(p => p.User).Where(x => !x.IsDeleted).ToListAsync();
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
                var entities = await _db.Opportunities.Include(f => f.KPIs).Include(p => p.User).Where(x => !x.IsDeleted && x.UserId == UserId).ToListAsync();
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
                    var entidad = _db.Opportunities.Include(f => f.KPIs).FirstOrDefault(o => o.Id == model.Id);

                    if (!string.IsNullOrEmpty(model.Picture))
                    {
                        entidad.Picture = model.Picture;
                    }
                    entidad.Title = model.Title;
                    entidad.Website = model.Website;
                    entidad.Description = model.Description;
                    entidad.LastUpdate = DateTime.Now;



                    //Vamos a editar unas cosillas :)
                    var alreadyInDB = _db.KPIs.Where(x => x.OpportunityId == model.Id).ToList();
                    var newToAdd = model.KPIs;
                    var newToNotDelete = new List<KPI>();

                    var martin = alreadyInDB.Where(x => !model.KPIs.Any(x2 => x2.Id == x.Id)).ToList();


                    //foreach (var item in model.KPIs)
                    //{
                    //    if (item.Id != null)
                    //        newToNotDelete.Add(alreadyInDB.FirstOrDefault(x => x.Id == item.Id));
                    //    else
                    //        newToNotDelete.Add(item);
                    //}

                    //var lista1 = newToNotDelete.Where(x => x.Id == null);
                    //var lista2 = newToNotDelete.Where(x => x.Id != null);

                    //Borramos los que ya no quieren
                    foreach (var item in martin)
                        item.IsDeleted = true;

                    //foreach (var item in model.KPIs)
                    //{
                    //    if (string.IsNullOrEmpty(item.Id.ToString()))
                    //    {
                    //        _db.AddRange(item);
                    //    }
                    //    else
                    //    {
                    //        continue;
                    //    }
                    //}

                    _db.AddRange(newToAdd.Where(x => x.Id == 0).ToList());

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

        [HttpPost("DeleteKPIS")]
        public async Task<IActionResult> DeleteKpis([FromBody]int KpiId)
        {
            if (string.IsNullOrEmpty(KpiId.ToString()))
            {
                var kpitodelete = await _db.KPIs.FirstOrDefaultAsync(x => x.Id == KpiId);
                kpitodelete.IsDeleted = true;
                _db.Entry(kpitodelete).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);
            }
            else
            {
                return BadRequest("Kpi id not found");
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
                      new User
                      {
                          Name = x.Name,
                          Naos = x.Naos,
                          ProfilePicUrl = x.ProfilePicUrl,

                      }
                ).ToListAsync();

                var listaordenada = users.OrderByDescending(x => Convert.ToInt32(x.Naos)).ToList();
                var position = 1;
                foreach (var item in listaordenada)
                {
                    item.Rank = position++;
                }

                return Ok(listaordenada);
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
