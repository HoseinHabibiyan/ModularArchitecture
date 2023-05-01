using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth;
using ModularArchitecture.Identity.Core;
using ModularArchitecture.MemoryCache;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using ModularArchitecture.Identity.Core.Inputs;
using ModularArchitecture.Identity.Core.Results;
using ModularArchitecture.Identity.Core.Extensions;
using ModularArchitecture.Identity.Core.Models;

namespace Security.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly UserManager _userManager;
        private readonly IConfiguration _config;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMemoryCacheService _memoryCacheService;

        public AccountsController(AuthenticationManager authenticationManager, UserManager userManager,
            IConfiguration config, SignInManager<ApplicationUser> signInManager, IMemoryCacheService memoryCacheService)
        {
            _authenticationManager = authenticationManager;
            _userManager = userManager;
            _config = config;
            _signInManager = signInManager;
            _memoryCacheService = memoryCacheService;
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var loginResult = await _authenticationManager.SignInJwtAsync(model);
            if (loginResult.Success)
            {
                return Ok(loginResult);
            }

            return BadRequest(loginResult.Message);
        }

        [HttpPost("registerLoginWithGoogle")]
        public async Task<IActionResult> RegisterLoginWithGoogle(GoogleToken token)
        {
            var validateAsync = await GoogleJsonWebSignature.ValidateAsync(token.Token);
            var email = validateAsync.Email;

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync("Google", token.Id, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                var signInJwtAsync = await _authenticationManager.ExternalSignInJwtAsync(new ExternalLoginModel
                {
                    ClientSecret = _config["ClientSecret"],
                    ClientId = _config["ClientId"],
                    ProviderKey = token.Id,
                    LoginProvider = "Google"
                });

                if (signInJwtAsync.Success)
                {
                    return Ok(new LoginResult((AuthToken)signInJwtAsync.Token, signInJwtAsync.User));
                }
            }

            if (result.IsNotAllowed)
            {
                return BadRequest("Your email is not confirmed");
            }

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                IsEnabled = true,
                EmailConfirmed = true,
                FirstName = token.FirstName,
                LastName = token.LastName
            };

            var identityResult = await _userManager.CreateAsync(user);

            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", token.Id, "Google"));
                if (identityResult.Succeeded)
                {
                    var signInJwtAsync = await _authenticationManager.ExternalSignInJwtAsync(new ExternalLoginModel
                    {
                        ClientSecret = _config["ClientSecret"],
                        ClientId = _config["ClientId"],
                        ProviderKey = token.Id,
                        LoginProvider = "Google"
                    });

                    if (signInJwtAsync.Success)
                    {
                        return Ok(new LoginResult((AuthToken)signInJwtAsync.Token, signInJwtAsync.User));
                    }

                    return Ok();
                }
            }
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(string token)
        {
            var result = await _authenticationManager.RefreshTokenAsync(token, _config["ClientId"], _config["ClientSecret"]);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var result = await _userManager.RegisterAsync(model);

            if (result.Succeeded)
            {
                _memoryCacheService.Remove(_config["CacheKey_Users"]);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result.GetErrors());
        }

        [HttpPost("ResetPasswordByAdmin")]
        [Authorize("Admin,SuperAdmin,Operator")]
        public async Task<IActionResult> ResetPasswordByAdmin(ResetPasswordByAdminModel model)
        {
            var result = await _userManager.ResetPasswordByAdminAsync(model);
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result.GetErrors());
        }

        [HttpPost("ForgetPassword")]
        public async Task<string> ForgetPassword(ForgetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        [HttpGet("ConfirmEmail/{userId:guid}/{code}")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result.GetErrors());
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result.GetErrors());
        }
    }

    public class GoogleToken
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
