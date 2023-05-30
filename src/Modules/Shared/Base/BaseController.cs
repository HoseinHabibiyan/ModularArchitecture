using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Base
{
    [Produces("application/json", new string[] { })]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BaseController : ControllerBase
    {
    }
}