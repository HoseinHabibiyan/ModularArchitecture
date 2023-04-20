using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModularArchitecture.Identity.Core;

namespace ModularArchitecture.Identity.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _appUserManager;
        private ModelFactory _modelFactory;
        private readonly RoleManager<ApplicationRole> _appRoleManager;
        private readonly ApplicationStore _applicationManager;

        public RolesController(RoleManager<ApplicationRole> appRoleManager, UserManager<ApplicationUser> appUserManager, ApplicationStore applicationManager)
        {
            _appRoleManager = appRoleManager;
            _appUserManager = appUserManager;
            _applicationManager = applicationManager;
        }

        protected ModelFactory ModelFactory => _modelFactory ??= new ModelFactory(_appUserManager);

        [Route("role/{id:guid}", Name = "GetRoleById")]
        public async Task<IActionResult> GetRole(string id)
        {
            var role = await _appRoleManager.FindByIdAsync(id);

            if (role != null)
            {
                return Ok(ModelFactory.Create(role));
            }

            return NotFound();
        }

        [Route("role/{name}", Name = "GetRoleByName")]
        public async Task<IActionResult> GetRoleByName(string name)
        {
            var role = await _appRoleManager.FindByNameAsync(name);

            if (role != null)
            {
                return Ok(ModelFactory.Create(role));
            }

            return NotFound();
        }

        [Route("")]
        public IActionResult GetAllRoles()
        {
            var roles = _appRoleManager.Roles.ToList();

            return Ok(roles);
        }

        [Route("create")]
        public async Task<IActionResult> Create(ICreateRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appId = _applicationManager.GetByName(model.ApplicationId).Id;
            var role = new ApplicationRole { Name = model.Name, ParentId = model.ParentId, ApplicationId = appId, SortOrder = model.SortOrder };

            var result = await _appRoleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            var locationHeader = new Uri(Url.Link("GetRoleById", new { id = role.Id }));

            return Created(locationHeader, ModelFactory.Create(role));

        }

        //[Authorize]
        [Route("update")]
        public async Task<IActionResult> Update(IUpdateRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = await _appRoleManager.FindByIdAsync(model.Id);

            role.ApplicationId = model.ApplicationId;
            role.Name = model.Name;
            role.SortOrder = model.SortOrder;
            role.ParentId = model.ParentId;

            try
            {
                var updateUserResult = await _appRoleManager.UpdateAsync(role);
                if (!updateUserResult.Succeeded)
                {
                    return GetErrorResult(updateUserResult);
                }

                var locationHeader = new Uri(Url.Link("GetRoleById", new { id = role.Id }));

                return Created(locationHeader, ModelFactory.Create(role));
            }
            catch (Exception e)
            {

                throw;
            }
        }

        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteRole(string id)
        {

            var role = await _appRoleManager.FindByIdAsync(id);

            if (role != null)
            {
                var result = await _appRoleManager.DeleteAsync(role);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();
            }

            return NotFound();

        }

        [Route("ManageUsersInRole")]
        public async Task<IActionResult> ManageUsersInRole(IUsersInRoleModel model)
        {
            var role = await _appRoleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist");
                return BadRequest(ModelState);
            }

            foreach (string user in model.EnrolledUsers)
            {
                var appUser = await _appUserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", $"User: {user} does not exists");
                    continue;
                }

                if (await _appUserManager.IsInRoleAsync(appUser, role.Name))
                    continue;

                var result = await _appUserManager.AddToRoleAsync(appUser, role.Name);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", $"User: {user} could not be added to role");
                }
            }

            foreach (string user in model.RemovedUsers)
            {
                var appUser = await _appUserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", $"User: {user} does not exists");
                    continue;
                }

                var result = await _appUserManager.RemoveFromRoleAsync(appUser, role.Name);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", $"User: {user} could not be removed from role");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        protected IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return StatusCode(500);
            }

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Error", error.Description);
                }
            }

            if (ModelState.IsValid)
            {
                // No ModelState errors are available to send, so just return an empty BadRequest.
                return BadRequest();
            }

            return BadRequest(ModelState);
        }
    }
}