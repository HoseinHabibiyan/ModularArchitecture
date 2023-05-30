using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularArchitecture.Identity.Abstraction;
using ModularArchitecture.Identity.Abstraction.Inputs;
using ModularArchitecture.Identity.Core.Models;
using ModularArchitecture.Identity.Core.Results;
using ModularArchitecture.Identity.EntityFramework;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;
namespace ModularArchitecture.Identity.Core
{
    public class UserManager : UserManager<ApplicationUser>
    {
        private readonly IdentityContext _identityContext;

        public UserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger, IdentityContext identityContext) :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
                services, logger)
        {
            _identityContext = identityContext;
        }

        public async Task<IdentityResult> RegisterAsync(IRegisterModel model)
        {
            var applicationUser = new ApplicationUser
            {
                Email = model.Email,
                FullName = model.FullName,
                MobileNumber = model.PhoneNumber,
                UserName = model.Username,
                IsEnabled = true,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                JoinDate = DateTime.Now,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                EnglishFullName = model.EnglishFullName
            };

            var result = await CreateAsync(applicationUser);

            if (!result.Succeeded)
                return result;

            IdentityResult addPasswordResult;
            try
            {
                addPasswordResult = await AddPasswordAsync(applicationUser, model.Password);
            }
            catch (Exception)
            {
                await DeleteAsync(applicationUser);
                throw;
            }

            if (addPasswordResult.Succeeded)
            {
                return IdentityResult.Success;
            }

            await DeleteAsync(applicationUser);
            return addPasswordResult;
        }

        public async Task<IdentityResult> ConfirmEmailSetPasswordAddToRolesAsync(IConfirmEmailSetPasswordAddToRolesModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Password and ConfirmPassword are different." });
            }
            var user = await FindByNameAsync(model.Email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User Not Found" });

            await using var transaction =
                await ((UserStore<ApplicationUser, ApplicationRole, IdentityContext, string, IdentityUserClaim<string>,
                    IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>,
                    IdentityRoleClaim<string>>)Store).Context.Database.BeginTransactionAsync();

            try
            {
                var result = await ConfirmEmailAsync(user, model.Token);

                if (!result.Succeeded)
                    return result;

                var addPasswordResult = await AddPasswordAsync(user, model.Password);
                if (addPasswordResult.Succeeded)
                {
                    var addToRolesResult = await AddToRolesAsync(user, model.Roles);
                    if (addToRolesResult.Succeeded)
                    {
                        await transaction.CommitAsync();
                        return IdentityResult.Success;
                    }

                    await transaction.RollbackAsync();
                    return addToRolesResult;
                }

                await transaction.RollbackAsync();
                return addPasswordResult;

            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                await transaction.RollbackAsync();
                return IdentityResult.Failed(new IdentityError { Description = e.Message });
            }
        }

        /// <summary>
        /// Finds and returns a list of <see cref="TextValue"/> of users, if any, who fullName contains the specified name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="name"/> if it exists.
        /// </returns>
        public async Task<List<TextValue>> GetUsersByNameAsync(string name)
        {
            name = name?.ToLower();
            var arr = name?.Split(' ');
            name = arr != null ? arr[0] : "";

            var list = await Users
                .Where(x => !string.IsNullOrEmpty(name) && x.FullName.ToLower().Contains(name))
                .Select(x => new TextValue { Text = x.FullName, Value = x.Id.ToString() }).ToListAsync();

            return list;
        }

        public async Task<List<ApplicationUser>> GetByNameAsync(string name)
        {
            name = name?.ToLower();
            var arr = name?.Split(' ');
            name = arr != null ? arr[0] : "";

            var list = await Users
                .Where(x => !string.IsNullOrEmpty(name) && x.FullName.ToLower().Contains(name))
                .ToListAsync();

            return list;
        }

        public async Task<List<ApplicationUser>> GetByUserNameAsync(string name)
        {
            var list = await Users
                .Where(x => !string.IsNullOrEmpty(name) && x.UserName.ToLower().Contains(name))
                .ToListAsync();

            return list;
        }

        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            var z = await Users.ToListAsync();
            return z;
        }

        public async Task<List<ApplicationUser>> GetUsersAsync(List<string> userIds)
        {
            var z = await Users.Where(x => userIds.Contains(x.Id)).ToListAsync();
            return z;
        }

        public async Task<List<UserRole>> GetUsersRolesAsync(List<string> userIds)
        {
            var context = ((UserStore<ApplicationUser, ApplicationRole, IdentityContext, string, IdentityUserClaim<string>,
                IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>,
                IdentityRoleClaim<string>>)Store).Context;

            var z = await context.UserRoles.Where(x => userIds.Contains(x.UserId)).Join(context.Roles, x => x.RoleId,
                x => x.Id, (x, y) => new UserRole { UserId = x.UserId, Role = y }).ToListAsync();
            return z;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="phone"/>.
        /// </summary>
        /// <param name="phone">The user PhoneNumber to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="phone"/> if it exists.
        /// </returns>
        public async Task<ApplicationUser> FindByPhoneAsync(string phone)
        {
            var user = await Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            return user;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="mobile"/>.
        /// </summary>
        /// <param name="mobile">The user MobileNumber to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="mobile"/> if it exists.
        /// </returns>
        public async Task<ApplicationUser> FindByMobileAsync(string mobile)
        {
            var user = await Users.FirstOrDefaultAsync(x => x.MobileNumber == mobile);
            return user;
        }

        /// <summary>
        /// Deletes the specified <paramref name="userId"/> from the backing store.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public async Task<IdentityResult> DeleteAsync(string userId)
        {
            ThrowIfDisposed();
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var user = await FindByIdAsync(userId);
            return await Store.DeleteAsync(user, CancellationToken);
        }

        public async Task<IdentityResult> ResetPasswordByAdminAsync(IResetPasswordByAdminModel model)
        {
            if (string.IsNullOrEmpty(model.Username)) model.Username = model.Email;
            var user = await FindByNameAsync(model.Username);
            user.PasswordHash = PasswordHasher.HashPassword(user, model.NewPassword);

            return await UpdateAsync(user);
        }

        /// <summary>
        /// Called to update the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Whether the operation was successful.</returns>
        public async Task<IdentityResult> UpdateUserAsync(IUpdateUserModel user)
        {
            var applicationUser = await FindByIdAsync(user.Id);

            applicationUser.FirstName = user.FirstName;
            applicationUser.MiddleName = user.MiddleName;
            applicationUser.LastName = user.LastName;
            applicationUser.FullName = user.FullName;
            applicationUser.PhoneNumber = user.PhoneNumber;
            applicationUser.Email = user.Email;
            applicationUser.LastLogin = user.LastLogin;
            applicationUser.MobileNumber = user.MobileNumber;
            applicationUser.ProfileImageUrl = user.ProfileImageUrl;
            applicationUser.EnglishFullName = user.EnglishFullName;

            return await base.UpdateAsync(applicationUser);
        }

        /// <summary>
        /// Called to enable the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>Whether the operation was successful.</returns>
        public async Task<IdentityResult> EnableUserAsync(string userId)
        {
            var applicationUser = await FindByIdAsync(userId);

            applicationUser.IsEnabled = true;

            return await base.UpdateAsync(applicationUser);
        }

        /// <summary>
        /// Called to disable the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>Whether the operation was successful.</returns>
        public async Task<IdentityResult> DisableUserAsync(string userId)
        {
            var applicationUser = await FindByIdAsync(userId);

            applicationUser.IsEnabled = false;

            return await base.UpdateAsync(applicationUser);
        }

        /// <summary>
        /// Called to enable users.
        /// </summary>
        /// <param name="userIds">The users id.</param>
        /// <returns>Whether the operation was successful.</returns>
        public async Task<IdentityResult> EnableUsersAsync(List<string> userIds)
        {
            var users = Users.Where(x => userIds.Contains(x.Id));

            await users.ForEachAsync(x => x.IsEnabled = true);
            try
            {
                await ((UserStore<ApplicationUser>)Store).Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Called to disable users.
        /// </summary>
        /// <param name="userIds">The users id.</param>
        /// <returns>Whether the operation was successful.</returns>
        public async Task<IdentityResult> DisableUsersAsync(List<string> userIds)
        {
            var users = Users.Where(x => userIds.Contains(x.Id));

            await users.ForEachAsync(x => x.IsEnabled = false);
            try
            {
                await ((UserStore<ApplicationUser>)Store).Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Called to remove Role from users.
        /// </summary>
        /// <param name="userIds">The users id.</param>
        /// <param name="role">Role to remove from Users</param>
        /// <returns>Whether the operation was successful.</returns>
        public async Task<IdentityResult> RemoveFromRoleAsync(List<string> userIds, string role)
        {
            var userRoles = _identityContext.Set<IdentityUserRole<string>>();
            var roles = _identityContext.Set<ApplicationRole>();
            var roleId = roles.FirstOrDefault(x => x.Name == role)?.Id;

            if (roleId == null) return IdentityResult.Success;

            var identityUserRoles = userRoles.Where(x => userIds.Contains(x.UserId) && x.RoleId == roleId);
            userRoles.RemoveRange(identityUserRoles);
            try
            {
                await _identityContext.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

        }
    }
}
