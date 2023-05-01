using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Base;
using System.Linq;
using System.Security.Claims;

namespace Security.Controllers
{
    public class ClaimsController : BaseController
    {
        [Authorize]
        [HttpGet]
        public IActionResult GetClaims()
        {
            var identity = User.Identity as ClaimsIdentity;

            var claims = from c in identity.Claims
                         select new
                         {
                             subject = c.Subject.Name,
                             type = c.Type,
                             value = c.Value
                         };

            return Ok(claims);
        }

    }
}