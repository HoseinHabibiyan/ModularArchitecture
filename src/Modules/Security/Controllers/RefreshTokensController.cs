using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModularArchitecture.Identity.EntityFramework;
using Shared.Base;

namespace Security.Controllers
{
    public class RefreshTokensController : BaseController
    {
        private readonly RefreshTokenStore _refreshTokenManager;

        public RefreshTokensController(RefreshTokenStore refreshTokenManager)
        {
            _refreshTokenManager = refreshTokenManager;
        }

        [Authorize(Roles = "Administrators")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_refreshTokenManager.GetAllRefreshTokens());
        }

        [Authorize(Roles = "Administrators")]
        [HttpDelete]
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
