using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FirebaseNet.Messaging;
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
        private readonly string  ServerApiKey = "AAAAN43ncTQ:APA91bEiKe1rDrG7xrICNrt7gSA1lMG7MDim1bPk1SKAvWnRIe7f6Dfm-XRdX85s5EnFNqRhRVfuL0CfVzH9ICEKNGT6PFNhyTb_JJz8lCZpbEH2meDi8QEZvlvjymJ7KS_TA779Rds4";

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

                    var x = await _db.Users.FindAsync(user.Id);     

                    x.FireBaseToken = model.FireBaseToken ?? x.FireBaseToken;
                    _db.Users.Update(x);
                    await _db.SaveChangesAsync();

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

                    // string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    // code = HttpUtility.UrlEncode(code);
                    // var callbackUrl = $"{Request.Host}/login/reset?code={code}&id={user.Id}";
                    // var sender = new EmailsService();
                    // var ca_correo = "contact@naomarketplace.com";
                    // sender.SendSimpleMessage(new Models.Email { Destination = ca_correo, Subject = "", Body = "" });

                    return Ok();
                }
                    return BadRequest();
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
                        return Ok();
                    else
                        return BadRequest("Invalid Credentials, please verify and try again.");
                }
                else
                    return BadRequest();
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

                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);

                if (user != null)
                {
                    user.Naos = user.Naos + 50;

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
                    var newList = new List<Match>();
                    var entidad = _db.Opportunities.FirstOrDefault(o => o.Id == model.Id);
                    entidad.IsDeleted = true;
                    entidad.LastUpdate = DateTime.Now;

                    _db.Entry(entidad).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    var Matches = _db.Matches.Where(x=>x.OpportunityId == entidad.Id);
                    if (Matches != null)
                    {
                        foreach (var item in Matches)
                        {
                            item.IsDeleted = true;
                            newList.Add(item);
                        }
                        _db.Matches.UpdateRange(newList);
                    }

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

                    return Ok(entities.OrderByDescending(x=>x.CreationDate));
                }
                else
                {
                    var entities = await _db.Opportunities.Include(f => f.KPIs).Include(p => p.User).Where(x => !x.IsDeleted).ToListAsync();
                    entities = entities.Select(x => { x.KPIs = x.KPIs.Where(y => !y.IsDeleted).ToList(); return x; }).ToList();
                    return Ok(entities.OrderByDescending(x=>x.CreationDate));
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
                return Ok(entities.OrderByDescending(x=>x.CreationDate));
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
                        entidad.Picture = model.Picture;
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
                            continue;
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

                    var Sender = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
                    var opp = await _db.Opportunities.Include(x=>x.KPIs).Include(u=>u.User).FirstOrDefaultAsync(x => x.Id == model.OpportunityId);
                    var Receiver = await _db.Users.FindAsync(opp.User.Id);

                    if (Sender != null && opp != null)
                    {
                        _db.Matches.Add(model);
                        Sender.Naos += 100;
                        Receiver.Naos += 100;
                        _db.Entry(Sender).State = EntityState.Modified;
                        _db.Entry(Receiver).State = EntityState.Modified;
                        await _db.SaveChangesAsync();

                        await Task.Run(async () => {
                            FCMClient client = new FCMClient(ServerApiKey); //as derived from https://console.firebase.google.com/project/
                            var message = new FirebaseNet.Messaging.Message()
                            {
                                To = Receiver.FireBaseToken, //topic example /topics/all
                                Notification = new IOSNotification()
                                {
                                    Body = "The user " + Sender.Name + " sent you a match.",
                                    Title = opp.Title + " Match!!",
                                },
                            };
                            var result = await client.SendMessageAsync(message);
                        });

                        return StatusCode(StatusCodes.Status200OK);
                    }
                    return BadRequest(ModelState);
                }
                else
                    return BadRequest("You’ve already sent a match to this opportunity and user.");
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("MatchBack")]
        public async Task<IActionResult> MatchBack([FromBody]Match model )
        {
            try
            {
                if (_db.Matches.Where(x => x.OpportunityId == model.OpportunityId && x.UserId == model.UserId).Count() == 1)
                {

                    var Receiver = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
                    var opp = await _db.Opportunities.Include(x=>x.KPIs).Include(u=>u.User).FirstOrDefaultAsync(x => x.Id == model.OpportunityId);
                    var Sender = await _db.Users.FindAsync(opp.User.Id);

                    var Match = await _db.Matches.FindAsync(model.Id);
                    if (Sender != null && opp != null && Receiver != null)
                    {
                        Match.LastUpdate = DateTime.Now;
                        Match.Status = Status.Matched;
                        _db.Matches.Update(Match);


                        Sender.Naos += 150;
                        Receiver.Naos += 150;

                        _db.Entry(Sender).State = EntityState.Modified;
                        _db.Entry(Receiver).State = EntityState.Modified;


                        await _db.SaveChangesAsync();

                        await Task.Run(async () => {
                            FCMClient client = new FCMClient(ServerApiKey); //as derived from https://console.firebase.google.com/project/
                            var message = new FirebaseNet.Messaging.Message()
                            {
                                To = Receiver.FireBaseToken, //topic example /topics/all
                                Notification = new IOSNotification()
                                {
                                    Body = "The user " + Sender.Name + " matched you back.",
                                    Title = opp.Title + " Matched back!",
                                },
                            };
                            var result = await client.SendMessageAsync(message);
                        });
                        return StatusCode(StatusCodes.Status200OK);
                    }
                    return BadRequest(ModelState);
                }
                else
                    return BadRequest("You already matched back this opportunity and user.");
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("GetSentMatches")]
        public async Task<IActionResult> GetSentMatches([FromQuery]string UserId)
        {
            var sentMatches = await _db.Matches.Where(x => x.UserId == UserId && x.Status == Status.Pending).ToListAsync();
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
            var user1opp = await _db.Opportunities.Include(u=>u.User).Where(x => !x.IsDeleted && (x.UserId == UserId1)).ToListAsync();

            List<Match> myMatches = new List<Match>();
            foreach (var item in user1opp)
            {
                myMatches.AddRange(_db.Matches.Include(u => u.User).Where(x => !x.IsDeleted && x.OpportunityId == item.Id).ToList());
            }
            var Matches = _db.Matches.Where(x=>x.Status == Status.Matched && x.UserId == UserId1).Include(u=>u.User).Include(o=>o.Opportunity).ThenInclude(u=>u.User);
            myMatches.AddRange(Matches);
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
                          Naos = x.Naos,
                          ProfilePicUrl = x.ProfilePicUrl,

                      }
                ).ToListAsync();

                var listaordenada = users.OrderByDescending(x => Convert.ToInt64(x.Naos)).ToList();
                var position = 1;
                foreach (var item in listaordenada)
                    item.Rank = position++;

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
                if (User1Id == User2Id)
                    return BadRequest("You cannot have a chat with yourself.");

                if (string.IsNullOrEmpty(User1Id) || string.IsNullOrEmpty(User1Id))
                    return BadRequest("We couldn't find the desired user, please try again later");

                var model = new Chat();
                model.User1 = await _db.Users.FirstOrDefaultAsync(x => x.Id == User1Id);
                model.User1Id = User1Id;
                model.User2 = await _db.Users.FirstOrDefaultAsync(x => x.Id == User2Id);
                model.User2Id = User2Id;

                if (model.User1 != null && model.User2 != null)
                {
                    var alreadyInDB = await _db.Chat.FirstOrDefaultAsync(x =>( x.User1Id == model.User1Id && x.User2Id == model.User2Id) ||
                     (x.User1Id == model.User2Id && x.User2Id == model.User1Id ));
                    //Not in DB so we can create the chat
                    if (alreadyInDB == null)
                    {
                        model.CreationDate = DateTime.UtcNow;
                        model.LastUpdate = DateTime.UtcNow;
                        _db.Chat.Add(model);
                        await _db.SaveChangesAsync();
                        return Ok(model);
                    }
                    alreadyInDB.Messages = _db.Message.OrderByDescending(y=>y.MessageDateTime).Where(y=>(y.Chat.User1Id == alreadyInDB.User1Id || y.Chat.User2Id == alreadyInDB.User2Id) && y.ChatId == alreadyInDB.Id).ToList();
                    return Ok(alreadyInDB);
                }
                return BadRequest("An error has occured while creating a new chat, please try again later");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("GetMyChats")]
        public async Task<IActionResult> GetMyChats([FromQuery]string UserId, string query)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x=>x.Id == UserId);
                var Mensajes = _db.Message;
                var Usuarios = _db.Users;

                if (user != null)
                {
                    if (!string.IsNullOrEmpty(query))
                    {
                        var Chats = await _db.Chat.Where(x=>(x.User1.Id == user.Id || x.User2.Id == user.Id) && (x.User1.Name.Contains(query) || x.User2.Name.Contains(query))).ToListAsync();
                        Chats = Chats.Select(x=>{
                        x.Messages = Mensajes.OrderByDescending(y=>y.MessageDateTime).Where(y=>(y.Chat.User1Id == UserId || y.Chat.User2Id == UserId) && y.ChatId == x.Id).ToList();
                        x.LastMessage =  Mensajes.OrderByDescending(y=>y.MessageDateTime).FirstOrDefault(y=>y.ChatId == x.Id);
                        x.User2 = Usuarios.FirstOrDefault(y=>y.Id == x.User2Id);
                        x.User1 = Usuarios.FirstOrDefault(y=>y.Id == x.User1Id);
                        return x;
                        }).ToList();
                        return Ok(Chats.OrderByDescending(x=>x.LastMessage.MessageDateTime));
                    }
                    else
                    {
                        var Chats = await _db.Chat.Where(x=>x.User1.Id == user.Id || x.User2.Id == user.Id).ToListAsync();
                        Chats = Chats.Select(x=>{
                        x.Messages = Mensajes.OrderByDescending(y=>y.MessageDateTime).Where(y=>(y.Chat.User1Id == UserId || y.Chat.User2Id == UserId) && y.ChatId == x.Id).ToList();
                        x.LastMessage =  Mensajes.OrderByDescending(y=>y.MessageDateTime).FirstOrDefault(y=>y.ChatId == x.Id);
                        x.User2 = Usuarios.FirstOrDefault(y=>y.Id == x.User2Id);
                        x.User1 = Usuarios.FirstOrDefault(y=>y.Id == x.User1Id);
                        return x;
                        }).ToList();
                        return Ok(Chats.OrderByDescending(x=>x.LastMessage.MessageDateTime));
                    }
                }
                return BadRequest("An error occured while loading your current chats, please try again later.");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody]Entities.Message sentMessage)
        {
            try
            {
                if (sentMessage != null)
                {
                    _db.Message.Add(sentMessage);
                    await _db.SaveChangesAsync();

                    var Receiver = await _db.Users.FindAsync(sentMessage.ReceiverId);
                    var Sender = await _db.Users.FindAsync(sentMessage.SenderId);

                    await Task.Run(async () => {

                        FCMClient client = new FCMClient(ServerApiKey); //as derived from https://console.firebase.google.com/project/

                        var message = new FirebaseNet.Messaging.Message()
                        {
                            To = Receiver.FireBaseToken, //topic example /topics/all
                            Notification = new IOSNotification()
                            {
                                Body = sentMessage.Text,
                                Title = Sender.Name,
                            },
                            Data = new Dictionary<string, string>
                            {
                                { "Body", sentMessage.Text },
                                { "Title", "New Message" },
                                { "SenderId", sentMessage.SenderId },
                                { "SenderPicture", Sender?.ProfilePicUrl}
                            }
                        };
                        var result = await client.SendMessageAsync(message);
                    });

                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        #endregion

        [HttpGet("GetNaos")]
        public async Task<IActionResult> GetNaos([FromQuery]string UserId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            if (user != null)
                return Ok(user.Naos);
            else
                return BadRequest("We couldn't load the " + user.Name + " naos, please try again later.");
        }
    }
}
