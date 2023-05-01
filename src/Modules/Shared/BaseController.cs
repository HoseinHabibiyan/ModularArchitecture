using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shared
{
    [Produces("application/json", new string[] { })]
    [ApiController]
    [Route("api/{culture}/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BaseController : ControllerBase
    {
    }
}