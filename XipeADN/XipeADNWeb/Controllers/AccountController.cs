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

        [HttpPost("EditProfile")]
        public async Task<IActionResult> EditProfile([FromBody]UserModel model)
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
                    entities = entities.Select(x => { x.KPIs = x.KPIs.Where(y => !y.IsDeleted).ToList(); return x; }).ToList();

                    return Ok(entities);
                }
                else
                {
                    var entities = await _db.Opportunities.Include(f => f.KPIs).Include(p => p.User).Where(x => !x.IsDeleted).ToListAsync();
                    entities = entities.Select(x => { x.KPIs = x.KPIs.Where(y => !y.IsDeleted).ToList(); return x; }).ToList();
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
                entities = entities.Select(x => { x.KPIs = x.KPIs.Where(y => !y.IsDeleted).ToList(); return x; }).ToList();
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

                    var martin = alreadyInDB.Where(x => !model.KPIs.Any(x2 => x2.Id == x.Id)).ToList();

                    //Borramos los que ya no quieren
                    foreach (var item in martin)
                        item.IsDeleted = true;

                    foreach (var item in newToAdd)
                    {
                        if (item.Id == 0)
                        {
                            item.OpportunityId = model.Id;
                            item.CreationDate = DateTime.Now;
                            item.LastUpdate = DateTime.Now;
                            item.IsDeleted = false;
                        }
                        else
                        {
                            continue;
                        }
                    }
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

        [HttpPost("SaveMatch")]
        public async Task<IActionResult> SaveMatch([FromBody]Match model )
        {
            try
            {
                if (_db.Matches.Where(x => x.OpportunityId == model.OpportunityId && x.UserId == model.UserId).Count() == 0)
                {
                    model.CreationDate = DateTime.Now;
                    model.LastUpdate = DateTime.Now;

                    var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
                    var opp = await _db.Opportunities.FirstOrDefaultAsync(x => x.Id == model.OpportunityId);

                    if (user != null && opp != null)
                    {
                        _db.Matches.Add(model);
                        await _db.SaveChangesAsync();

                        return StatusCode(StatusCodes.Status200OK);
                    }
                    return BadRequest(ModelState);
                }
                else
                {
                    return BadRequest("You’ve already sent a match to this opportunity and user.");
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("GetSentMatches")]
        public async Task<IActionResult> GetSentMatches([FromQuery]string UserId)
        {
            var sentMatches = await _db.Matches.Where(x => x.UserId == UserId).ToListAsync();
            List<Match> newList = new List<Match>();

            foreach (var item in sentMatches)
            {
                if (!newList.Select(x=>x.OpportunityId).Contains(item.OpportunityId))
                {
                    item.Opportunity = _db.Opportunities.Include(u=>u.User).FirstOrDefault(x => x.Id == item.OpportunityId);
                    item.User = _db.Users.FirstOrDefault(x => x.Id == item.UserId);
                    newList.Add(item);
                }
            }
            return Ok(newList);
        }

        [HttpGet("GetMatches")]
        public async Task<IActionResult> GetMatches([FromQuery]string UserId1)
        {
            var user1opp = await _db.Opportunities.Include(u=>u.User).Where(x => !x.IsDeleted && x.UserId == UserId1).ToListAsync();

            List<Match> myMatches = new List<Match>();
            foreach (var item in user1opp)
            {
                myMatches.AddRange(_db.Matches.Include(u => u.User).Where(x => !x.IsDeleted && x.OpportunityId == item.Id).ToList());
            }

            return Ok(myMatches);
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
                          Naos = x.Naos ?? "0",
                          ProfilePicUrl = x.ProfilePicUrl,

                      }
                ).ToListAsync();

                var listaordenada = users.OrderByDescending(x => Convert.ToInt64(x.Naos)).ToList();
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

        [HttpPost("StartChat")]
        public async Task<IActionResult> StartChat([FromQuery]string User1Id, string User2Id)
        {
            try
            {         

                if (string.IsNullOrEmpty(User1Id) || string.IsNullOrEmpty(User1Id))
                {
                    return BadRequest("We couldn't find the desired user, please try again later");
                }

                var model = new Chat();
                model.User1 = await _db.Users.FirstOrDefaultAsync(x => x.Id == User1Id);
                model.User1Id = User1Id;
                model.User2 = await _db.Users.FirstOrDefaultAsync(x => x.Id == User2Id);
                model.User2Id = User2Id;

                if (model.User1 != null && model.User2 != null)
                {
                    var alreadyInDB = await _db.Chat.FirstOrDefaultAsync(x => x.User1Id == model.User1Id && x.User2Id == model.User2Id);
                    //Not in DB so we can create the chat
                    if (alreadyInDB == null)
                    {
                        model.CreationDate = DateTime.UtcNow;
                        model.LastUpdate = DateTime.UtcNow;
                        _db.Chat.Add(model);
                        await _db.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK);
                    }
                    return BadRequest("You already have a chat with this user.");
                    
                }
                return BadRequest("An error has occured while creating a new chat, please try again later");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("GetMyChats")]
        public async Task<IActionResult> GetMyChats([FromQuery]string UserId)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x=>x.Id == UserId);

                if (user != null)
                {
                    var Chats = await _db.Chat.Where(x=>x.User1.Id == user.Id || x.User2.Id == user.Id).ToListAsync();
                    Chats = Chats.Select(x=>{
                        x.LastMessage =  _db.Message.OrderByDescending(y=>y.MessageDateTime).FirstOrDefault(y=>y.ChatId == x.Id);
                        x.User2 = _db.Users.FirstOrDefault(y=>y.Id == x.User2Id);
                        x.User1 = _db.Users.FirstOrDefault(y=>y.Id == x.User1Id);
                        return x;
                    }).ToList();
                    return Ok(Chats);
                }
                return BadRequest("An error occured while loading your current chats, please try again later.");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

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
