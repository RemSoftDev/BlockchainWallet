using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Wallet.Helpers;
using Wallet.Models;
using Wallet.ViewModels;

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JWTSettings _jwtOptions;

        public AccountController(UserManager<User> userManager, IJwtFactory jwtFactory,
            IOptions<JWTSettings> jwtOptions)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User user = new User() { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(HttpErrorHandler.AddErrors(result, ModelState));

            string confirmUrl =
                GetEmailConfirmationUrl(user, await _userManager.GenerateEmailConfirmationTokenAsync(user));

            try
            {
                await EmailHelper.SendEmailAsync(user.Email, "Email confirmation",
                    EmailHelper.GetEmailConfirmationMessage(confirmUrl));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Can't sent email");
            }

            return new OkResult();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LogIn([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    return BadRequest(HttpErrorHandler.AddError("Failure", "User not found", ModelState));
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return BadRequest(HttpErrorHandler.AddError("Failure", "Confirm your email", ModelState));
                }

                var result = await GetClaimsIdentity(model.Email, model.Password);

                if (result == null)
                    return BadRequest(HttpErrorHandler.AddError("Failure", "Invalid username or password.",
                        ModelState));
                var jwt = await Tokens.GenerateJwt(result, _jwtFactory, model.Email, _jwtOptions,
                    new JsonSerializerSettings {Formatting = Formatting.Indented});

                return new OkObjectResult(jwt);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"En error occurred :{e.Message}");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return BadRequest(HttpErrorHandler.AddError("Failure", "Confirm your email", ModelState));
                }

                string resetUrl = GetPasswordResetUrl(user, await _userManager.GeneratePasswordResetTokenAsync(user));

                await EmailHelper.SendEmailAsync(user.Email, "Reset Password",
                    EmailHelper.GetResetPasswordMessage(resetUrl));

                return new OkResult();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"En error occurred :{e.Message}");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                    return BadRequest(HttpErrorHandler.AddError("Failure", "User not found", ModelState));

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (!result.Succeeded)
                    return BadRequest(HttpErrorHandler.AddErrors(result, ModelState));

                return new OkResult();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"En error occurred :{e.Message}");
            }
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);       
            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            if (await _userManager.IsInRoleAsync(userToVerify, "Admin"))
            {
                if (await _userManager.CheckPasswordAsync(userToVerify, password))
                {
                    return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id, true));
                }
            }
            else
            {
                if (await _userManager.CheckPasswordAsync(userToVerify, password))
                {
                    return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id, false));
                }
            }       

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ConfirmEmail([FromBody]ConfirmEmailViewModel model)
        {
            if (model.UserId == null || model.Code == null)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return BadRequest();
            var result = await _userManager.ConfirmEmailAsync(user, model.Code);
            if (result.Succeeded)
            {
                return new OkResult();
            }
            else
            {
                return BadRequest("Error");
            }
        }

        private string GetPasswordResetUrl(User user, string code)
        {
            return Url.Action("ResetPassword", "Account", new {userId = user.Id, code = code},
                protocol: HttpContext.Request.Scheme);
        }

        private string GetEmailConfirmationUrl(User user, string code)
        {
            return Url.Action(
                "ConfirmEmail",
                "Account",
                new {userId = user.Id, code = code},
                protocol: HttpContext.Request.Scheme);
        }
    }
}