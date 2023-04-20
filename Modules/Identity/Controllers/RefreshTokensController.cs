using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using  ModularArchitecture.Identity.Core;

namespace ModularArchitecture.Identity.Core.Controllers
{
    [ApiController]
    [Route("api/refreshtoken")]
    public class RefreshTokensController : ControllerBase
    {
        private readonly RefreshTokenStore _refreshTokenManager;

        public RefreshTokensController(RefreshTokenStore refreshTokenManager)
        {
            _refreshTokenManager = refreshTokenManager;
        }

        [Authorize(Roles = "Administrators")]
        [Route("")]
        public IActionResult Get()
        {
            return Ok(_refreshTokenManager.GetAllRefreshTokens());
        }

        [Authorize(Roles = "Administrators")]
        //[AllowAnonymous]
        [Route("")]
        public async Task<IActionResult> Delete(string tokenId)
        {
            var result = await _refreshTokenManager.RemoveRefreshTokenAsync(tokenId);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Token Id does not exist");
        }
    }
}
