
using Microsoft.AspNetCore.Identity;

namespace  ModularArchitecture.Identity.Core
{
    public class ModelFactory
    {
        private readonly UserManager<ApplicationUser> _appUserManager;

        public ModelFactory(UserManager<ApplicationUser> appUserManager)
        {
            _appUserManager = appUserManager;
        }

        public User Create(ApplicationUser appUser)
        {
            return new User
            {
                Id = appUser.Id,
                Code = appUser.Code,
                UserName = appUser.UserName,
                FullName = appUser.FullName,
                FirstName = appUser.FirstName,
                MiddleName = appUser.MiddleName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                MobileNumber = appUser.MobileNumber,
                Level = appUser.Level,
                JoinDate = appUser.JoinDate,
                Roles = _appUserManager.GetRolesAsync(appUser).Result.ToList(),
                IsEnabled = appUser.IsEnabled,
                IsDeleted = appUser.IsDeleted,
                LastLogin = appUser.LastLogin,
                IsSuperUser = appUser.IsSuperUser,
                ProfileImageUrl = appUser.ProfileImageUrl,
            };
        }

        public Role Create(ApplicationRole appRole)
        {
            return new Role
            {
                Id = appRole.Id,
                Name = appRole.Name,
                SortOrder = appRole.SortOrder,
                ParentId = appRole.ParentId
            };
        }
    }
}