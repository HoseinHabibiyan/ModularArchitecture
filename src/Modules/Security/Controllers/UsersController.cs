using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModularArchitecture.Identity.Abstraction.Inputs;
using ModularArchitecture.Identity.Core;
using ModularArchitecture.Identity.Core.Extensions;
using ModularArchitecture.Identity.Core.Inputs;
using ModularArchitecture.Identity.Core.Models;
using System.Security.Claims;

namespace Security.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _appUserManager;
        private ModelFactory _modelFactory;
        private readonly RoleManager<ApplicationRole> _appRoleManager;
        protected ModelFactory ModelFactory => _modelFactory ??= new ModelFactory(_appUserManager);

        public UsersController(UserManager<ApplicationUser> appUserManager, RoleManager<ApplicationRole> appRoleManager)
        {
            _appUserManager = appUserManager;
            _appRoleManager = appRoleManager;
        }

        [HttpGet("GetCurrentUser")]
        [Authorize]
        public async Task<ApplicationUser> GetCurrentUser()
        {
            var claims = Request.HttpContext.User.Claims.ToList();
            var userId = claims.Any() ? claims.First(i => i.Type == ClaimTypes.NameIdentifier)?.Value : null;
            return await _appUserManager.FindByIdAsync(userId);
        }

        [Route("")]
        public IActionResult GetUsers()
        {
            var z = _appUserManager.Users.ToList();
            return Ok(z);
        }

        [Route("users")]
        [HttpPost]
        public IActionResult GetUsers(List<string> userIds)
        {
            var z = _appUserManager.Users.Where(x => userIds.Contains(x.Id)).ToList();
            return Ok(z);
        }

        [Route("usersInRole/{roleName}")]
        public async Task<IActionResult> GetUsersInRole(string roleName)
        {
            var users = await _appUserManager.GetUsersInRoleAsync(roleName);
            return Ok(users);
        }

        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _appUserManager.FindByIdAsync(id);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        //[Authorize]
        [Route("user/{username}/")]
        public async Task<IActionResult> GetUserByName(string username)
        {
            var user = await _appUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();

        }

        [Route("user/phone/{phone}/")]
        public IActionResult GetUserByPhone(string phone)
        {
            var user = _appUserManager.Users.FirstOrDefault(x => x.PhoneNumber == phone);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();

        }

        [Route("users/{name}/")]
        public IActionResult GetUsersByName(string name)
        {
            name = name?.ToLower();
            var arr = name?.Split(' ');
            name = arr != null ? arr[0] : "";

            var list = _appUserManager.Users
                .Where(x => !string.IsNullOrEmpty(name) && x.FullName.ToLower().Contains(name))
                .Select(x => new { Text = x.FullName, Value = x.Id.ToString() }).ToList();

            return Ok(list);
        }

        [AllowAnonymous]
        [Route("create")]
        public async Task<IActionResult> CreateUser(RegisterModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FullName = createUserModel.FullName,
                FirstName = createUserModel.FirstName,
                MiddleName = createUserModel.MiddleName,
                LastName = createUserModel.LastName,
                PhoneNumber = createUserModel.PhoneNumber,
                Level = 3,
                JoinDate = DateTime.Now,
                IsEnabled = true
            };

            var addUserResult = await _appUserManager.CreateAsync(user, createUserModel.Password);
            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            var locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, user);
        }

        [AllowAnonymous]
        [Route("GenerateEmailConfirmationToken")]
        public async Task<string> GenerateEmailConfirmationToken(string userId)
        {
            var user = await _appUserManager.FindByIdAsync(userId);
            var code = await _appUserManager.GenerateEmailConfirmationTokenAsync(user);
            return code;
        }

        [Authorize]
        [Route("update")]
        public async Task<IActionResult> UpdateUser(IUpdateUserModel updateUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _appUserManager.FindByIdAsync(updateUserModel.Id);

            user.Email = updateUserModel.Email;
            user.FullName = updateUserModel.FullName;
            user.FirstName = updateUserModel.FirstName;
            user.MiddleName = updateUserModel.MiddleName;
            user.LastName = updateUserModel.LastName;
            user.PhoneNumber = updateUserModel.PhoneNumber;
            user.LastLogin = updateUserModel.LastLogin;
            user.ProfileImageUrl = updateUserModel.ProfileImageUrl;

            var updateUserResult = await _appUserManager.UpdateAsync(user);
            return !updateUserResult.Succeeded ? GetErrorResult(updateUserResult) : Ok(user);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [Route("UpdateUserByEmail")]
        public async Task<IActionResult> UpdateUserByEmail(IUpdateUserModel updateUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _appUserManager.FindByNameAsync(updateUserModel.Email);

            user.FullName = updateUserModel.FullName;
            user.FirstName = updateUserModel.FirstName;
            user.MiddleName = updateUserModel.MiddleName;
            user.LastName = updateUserModel.LastName;
            user.PhoneNumber = updateUserModel.PhoneNumber;
            user.LastLogin = updateUserModel.LastLogin;
            user.ProfileImageUrl = updateUserModel.ProfileImageUrl;

            var updateUserResult = await _appUserManager.UpdateAsync(user);
            return !updateUserResult.Succeeded ? GetErrorResult(updateUserResult) : Ok(user);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            var user = await _appUserManager.FindByIdAsync(userId);
            code = code.Replace(" ", "+");
            var result = await _appUserManager.ConfirmEmailAsync(user, code);

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        [Authorize]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(IChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _appUserManager.FindByIdAsync(User.Identity.GetUserId());
            var result = await _appUserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            return !result.Succeeded ? GetErrorResult(result) : Ok();
        }

        [Authorize(Roles = "Admin,SuperAdmin,Operators")]
        [Route("ResetPasswordByAdmin")]
        public async Task<IActionResult> ResetPasswordByAdmin(IResetPasswordByAdminModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _appUserManager.FindByNameAsync(model.Email);
            user.PasswordHash = _appUserManager.PasswordHasher.HashPassword(user, model.NewPassword);

            var result = await _appUserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [Route("GeneratePasswordResetToken")]
        public async Task<IActionResult> GeneratePasswordResetToken(IForgetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _appUserManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return NotFound();
            }

            var code = await _appUserManager.GeneratePasswordResetTokenAsync(user);

            return Ok(code);
        }

        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(IResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _appUserManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return NotFound();
            }
            var code = model.Code.Replace(" ", "+");
            var result = await _appUserManager.ResetPasswordAsync(user, code, model.Password);

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [Route("user/{id:guid}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var appUser = await _appUserManager.FindByIdAsync(id);

            if (appUser == null)
                return NotFound();

            var result = await _appUserManager.DeleteAsync(appUser);

            return !result.Succeeded ? GetErrorResult(result) : Ok();
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [Route("LockUser/{id:guid}")]
        public async Task<IActionResult> LockUser(string id)
        {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)

            var appUser = await _appUserManager.FindByIdAsync(id);

            if (appUser == null)
                return NotFound();

            var result = await _appUserManager.SetLockoutEnabledAsync(appUser, true);

            return !result.Succeeded ? GetErrorResult(result) : Ok();
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [Route("UnLockUser/{id:guid}")]
        public async Task<IActionResult> UnLockUser(string id)
        {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)

            var appUser = await _appUserManager.FindByIdAsync(id);

            if (appUser == null)
                return NotFound();

            var result = await _appUserManager.SetLockoutEnabledAsync(appUser, false);

            return !result.Succeeded ? GetErrorResult(result) : Ok();
        }

        [Route("user/{id:guid}/roles")]
        [HttpPut]
        public async Task<IActionResult> AssignRolesToUser([FromRoute] string id, [FromBody] string[] rolesToAssign)
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            if (!isAdmin)
            {
                if (rolesToAssign.Contains("Admin") || rolesToAssign.Contains("SuperAdmin"))
                {
                    return Unauthorized();
                }
            }

            var appUser = await _appUserManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            var currentRoles = await _appUserManager.GetRolesAsync(appUser);

            var rolesNotExists = rolesToAssign.Except(_appRoleManager.Roles.Select(x => x.Name)).ToArray();

            if (rolesNotExists.Any())
            {

                ModelState.AddModelError("", $"Roles '{string.Join(",", rolesNotExists)}' does not exists in the system");
                return BadRequest(ModelState);
            }

            var removeResult = await _appUserManager.RemoveFromRolesAsync(appUser, currentRoles.ToArray());

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return BadRequest(ModelState);
            }

            var addResult = await _appUserManager.AddToRolesAsync(appUser, rolesToAssign);

            if (addResult.Succeeded)
                return Ok();

            ModelState.AddModelError("", "Failed to add user roles");
            return BadRequest(ModelState);
        }

        [Authorize]
        [Route("user/{id:guid}/removeroles")]
        [HttpPut]
        public async Task<IActionResult> RemoveRolesFromUser([FromRoute] string id, [FromBody] string[] rolesToRemove)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = await _appUserManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            foreach (var role in rolesToRemove)
            {
                var result = await _appUserManager.RemoveFromRoleAsync(appUser, role);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", $"Role: {role} could not be removed from user");
                }
            }

            return Ok();
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [Route("user/{id:guid}/assignclaims")]
        [HttpPut]
        public async Task<IActionResult> AssignClaimsToUser([FromRoute] string id, [FromBody] List<ClaimBindingModel> claimsToAssign)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = await _appUserManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            await _appUserManager.AddClaimsAsync(appUser, claimsToAssign.Select(x => new Claim(x.Type, x.Value)));

            return Ok();
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [Route("user/{id:guid}/removeclaims")]
        [HttpPut]
        public async Task<IActionResult> RemoveClaimsFromUser([FromRoute] string id, [FromBody] List<ClaimBindingModel> claimsToRemove)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = await _appUserManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            await _appUserManager.RemoveClaimsAsync(appUser, claimsToRemove.Select(x => new Claim(x.Type, x.Value)));

            return Ok();
        }

        //[Route("celltoken/{cell}")]
        //[HttpGet]
        //public async Task<IActionResult> GenerateCellToken(string cell)
        //{
        //    if (string.IsNullOrEmpty(cell))
        //    {
        //        return BadRequest("Cell number should be sent.");
        //    }
        //    var random = new Random();
        //    var syncLock = new object();
        //    int code;
        //    lock (syncLock)
        //    {
        //        // synchronize
        //        code = random.Next(1000, 9999);
        //    }

        //    var cellToken = new CellToken(cell, code);
        //    await _repo.AddCellVerificationAsync(cellToken);
        //    return Ok(cellToken);
        //}

        //[Route("celltoken/verify")]
        //public async Task<IActionResult> VerifyCellToken(VerifyCellTokenBindingModel token)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }

        //    CellToken cellToken = _repo.GetCellToken(token.Cell);
        //    if (cellToken != null && cellToken.Code.ToString() == token.Code)
        //    {
        //        await _repo.RemoveCellTokenAsync(cellToken);
        //        return Ok(cellToken);
        //    }

        //    return BadRequest("code is invalid");
        //}

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [Route("EnableUser/{id:guid}")]
        public async Task<IActionResult> EnableUser(string id)
        {
            var appUser = await _appUserManager.FindByIdAsync(id);

            if (appUser == null)
                return NotFound();

            appUser.IsEnabled = true;
            var result = await _appUserManager.UpdateAsync(appUser);

            return !result.Succeeded ? GetErrorResult(result) : Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [Route("DisableUser/{id:guid}")]
        public async Task<IActionResult> DisableUser(string id)
        {
            var appUser = await _appUserManager.FindByIdAsync(id);

            if (appUser == null)
                return NotFound();

            appUser.IsEnabled = false;
            var result = await _appUserManager.UpdateAsync(appUser);

            return !result.Succeeded ? GetErrorResult(result) : Ok();
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
