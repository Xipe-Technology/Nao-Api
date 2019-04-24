using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XipeADNWeb.Models;
using XipeADNWeb.Services;

namespace XipeADNWeb.Controllers
{
    [ /*Authorize, */ Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService) =>
            _userService = userService;

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
                await _userService.EditProfile(new UserModel { ProfilePictureUrl = upload });
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
    }
}
