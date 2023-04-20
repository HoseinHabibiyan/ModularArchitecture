using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using ModularArchitecture.Identity.Abstraction.Results;
using ModularArchitecture.Identity.Core.Inputs;
using ModularArchitecture.Identity.Core.Models;
using ModularArchitecture.Identity.Core.Results;
using ModularArchitecture.Identity.EntityFramework;
using ModularArchitecture.Identity.Jwt;
using IdentityResult = ModularArchitecture.Identity.Core.Results.IdentityResult;

namespace ModularArchitecture.Identity.Core
{
    public class AuthenticationManager
    {
        private readonly ClientStore _clientManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsPrincipalFactory;
        private ModelFactory _modelFactory;
        private readonly TokenManager _tokenManager;
        private readonly RefreshTokenStore _refreshTokenStore;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IStringLocalizer _localizer;

        protected ModelFactory ModelFactory => _modelFactory ??= new ModelFactory(_userManager);

        public AuthenticationManager(ClientStore clientManager, UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory, TokenManager tokenManager, RefreshTokenStore refreshTokenStore, SignInManager<ApplicationUser> signInManager, IStringLocalizer localizer)
        {
            _clientManager = clientManager;
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _tokenManager = tokenManager;
            _refreshTokenStore = refreshTokenStore;
            _signInManager = signInManager;
            _localizer = localizer;
        }

        public async Task<ILoginResult> SignInJwtAsync(LoginModel model)
        {
            if (model.ClientId == null)
            {
                return new LoginResult(_localizer["ClientId should be sent."]);
            }

            var client = _clientManager.FindClient(model.ClientId);

            var validateClientResult = ValidateClient(client, model.ClientId, model.ClientSecret);

            if (!validateClientResult.Success)
            {
                return new LoginResult(validateClientResult.Message);
            }

            var user = await _userManager.FindByNameAsync(model.Username);

            var validateUserResult = ValidateUser(user);

            if (!validateUserResult.Success)
            {
                return new LoginResult(validateUserResult.Message);
            }

            //var checkPasswordResult = await _userManager.CheckPasswordAsync(user, model.Password);

            //if (!checkPasswordResult)
            //{
            //    return new LoginResult("Username or Password is invalid");
            //}

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return new LoginResult(_localizer["This account is locked out"]);
                }

                if (signInResult.IsNotAllowed)
                {
                    return new LoginResult(_localizer["Login is not allowed"]);
                }

                if (signInResult.RequiresTwoFactor)
                {
                    return new LoginResult(_localizer["Requires Two Factor"]);
                }

                return new LoginResult(_localizer["Username or Password is invalid"]);
            }

            var claimsIdentity = await CreateClaimsIdentity(user, model.Claims);

            var refreshTokenId = Guid.NewGuid().ToString("n");

            var jwtToken = _tokenManager.GenerateJwtToken(claimsIdentity, client.Id, client.Secret, client.TokenLifeTime);
            var refreshToken = _tokenManager.GenerateRefreshToken(claimsIdentity, refreshTokenId, client.Id, client.RefreshTokenLifeTime);

            await _refreshTokenStore.RemoveOldTokensAsync(user.UserName);
            await _refreshTokenStore.AddRefreshTokenAsync(refreshToken);

            user.LastLogin = DateTime.Now;
            await _userManager.UpdateAsync(user);

            return new LoginResult(new AuthToken { AccessToken = jwtToken, RefreshToken = refreshTokenId }, ModelFactory.Create(user));
        }

        public async Task<ILoginResult> ExternalSignInJwtAsync(ExternalLoginModel model)
        {
            if (model.ClientId == null)
            {
                return new LoginResult("ClientId should be sent.");
            }

            var client = _clientManager.FindClient(model.ClientId);

            var validateClientResult = ValidateClient(client, model.ClientId, model.ClientSecret);

            if (!validateClientResult.Success)
            {
                return new LoginResult(validateClientResult.Message);
            }

            var user = await _userManager.FindByLoginAsync(model.LoginProvider, model.ProviderKey);

            var validateUserResult = ValidateUser(user);

            if (!validateUserResult.Success)
            {
                return new LoginResult(validateUserResult.Message);
            }

            var claimsIdentity = await CreateClaimsIdentity(user, model.Claims);

            var refreshTokenId = Guid.NewGuid().ToString("n");

            var jwtToken = _tokenManager.GenerateJwtToken(claimsIdentity, client.Id, client.Secret, client.TokenLifeTime);
            var refreshToken = _tokenManager.GenerateRefreshToken(claimsIdentity, refreshTokenId, client.Id, client.RefreshTokenLifeTime);

            await _refreshTokenStore.RemoveOldTokensAsync(user.UserName);
            await _refreshTokenStore.AddRefreshTokenAsync(refreshToken);

            return new LoginResult(new AuthToken { AccessToken = jwtToken, RefreshToken = refreshTokenId }, ModelFactory.Create(user));
        }

        public async Task<IRefreshTokenResult> RefreshTokenAsync(string token, string clientId, string clientSecret, List<Claim> claims = null)
        {
            string hashedTokenId = _tokenManager.RandomTokenString(token);
            var refreshToken = await _refreshTokenStore.FindRefreshTokenAsync(hashedTokenId);

            if (refreshToken == null)
            {
                return new RefreshTokenResult("Not Found");
            }

            if (refreshToken.ExpiresUtc < DateTime.UtcNow)
            {
                await _refreshTokenStore.RemoveRefreshTokenAsync(refreshToken);
                return new RefreshTokenResult("Token Expired");
            }

            if (clientId == null)
            {
                return new RefreshTokenResult("ClientId should be sent.");
            }

            var client = _clientManager.FindClient(clientId);

            var validateClientResult = ValidateClient(client, clientId, clientSecret);

            if (!validateClientResult.Success)
            {
                return new RefreshTokenResult(validateClientResult.Message);
            }

            var user = await _userManager.FindByNameAsync(refreshToken.Subject);

            var validateUserResult = ValidateUser(user);

            if (!validateUserResult.Success)
            {
                return new RefreshTokenResult(validateUserResult.Message);
            }

            var claimsIdentity = await CreateClaimsIdentity(user, claims);

            var refreshTokenId = Guid.NewGuid().ToString("n");

            var jwtToken = _tokenManager.GenerateJwtToken(claimsIdentity, client.Id, client.Secret, client.TokenLifeTime);
            var newRefreshToken = _tokenManager.GenerateRefreshToken(claimsIdentity, refreshTokenId, client.Id, client.RefreshTokenLifeTime);

            await _refreshTokenStore.RemoveOldTokensAsync(user.UserName);
            await _refreshTokenStore.AddRefreshTokenAsync(newRefreshToken);

            return new RefreshTokenResult(new AuthToken { AccessToken = jwtToken, RefreshToken = refreshTokenId }) { User = ModelFactory.Create(user) };
        }

        public async Task<ApplicationUser> GetRefreshTokenSubjectAsync(string token)
        {
            string hashedTokenId = _tokenManager.RandomTokenString(token);
            var refreshToken = await _refreshTokenStore.FindRefreshTokenAsync(hashedTokenId);

            if (refreshToken == null)
            {
                return null;
            }
            return await _userManager.FindByNameAsync(refreshToken.Subject);
        }

        public async Task RevokeTokenAsync(string token)
        {
            var hashedTokenId = _tokenManager.RandomTokenString(token);
            await _refreshTokenStore.RemoveRefreshTokenAsync(hashedTokenId);
        }

        private async Task<ClaimsIdentity> CreateClaimsIdentity(ApplicationUser user, List<Claim> claims)
        {
            var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(user);
            var claimsIdentity = (ClaimsIdentity)claimsPrincipal.Identity;
            claimsIdentity.AddClaim(new Claim("IsEnabled", user.IsEnabled.ToString()));
            claimsIdentity.AddClaim(new Claim("IsDeleted", user.IsDeleted.ToString()));
            claimsIdentity.AddClaim(new Claim("IsSuperUser", user.IsSuperUser.ToString()));
            if (user.MustChangePassword)
                claimsIdentity.AddClaim(new Claim("MustChangePassword", user.MustChangePassword.ToString()));

            if (!string.IsNullOrEmpty(user.FullName))
                claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.FullName));
            if (!string.IsNullOrEmpty(user.PhoneNumber))
                claimsIdentity.AddClaim(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
            if (!string.IsNullOrEmpty(user.Email))
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                claimsIdentity.AddClaim(new Claim("ImageUrl", user.ProfileImageUrl));

            if (claims == null) return claimsIdentity;

            foreach (var claim in claims)
            {
                claimsIdentity.AddClaim(claim);
            }

            return claimsIdentity;
        }

        private IIdentityResult ValidateUser(ApplicationUser user)
        {
            if (user == null)
            {
                return new IdentityResult("Username or Password is invalid.");
            }

            if (!user.IsEnabled)
            {
                return new IdentityResult("User had Disabled");
            }

            if (user.IsDeleted)
            {
                return new IdentityResult("Account has been deleted.");
            }

            return new IdentityResult { Success = true };
        }

        private IIdentityResult ValidateClient(Client client, string clientId, string clientSecret)
        {
            if (client == null)
            {
                return new IdentityResult($"Client '{clientId}' is not registered in the system.");
            }

            if (client.ApplicationType == ApplicationTypes.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    return new IdentityResult("Client secret should be sent.");
                }

                if (client.Secret != clientSecret)
                {
                    return new IdentityResult("Client secret is invalid.");
                }
            }

            if (!client.Active)
            {
                return new IdentityResult("Client is inactive.");
            }

            return new IdentityResult { Success = true };
        }
    }
}
