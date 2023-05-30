using ModularArchitecture.Identity.Abstraction.Inputs;
using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Abstraction
{
    public interface IAccountService
    {
        Task<ILoginResult> LoginAsync(ILoginModel login);
        Task<IRefreshTokenResult> RefreshTokenAsync(string refreshToken);
        Task<IGenerateRegistrationCodeResult> GenerateRegistrationCodeAsync(string cell);
        Task<IIdentityResult> VerifyRegistrationCodeAsync(IVerifyRegistrationCodeModel model);
        Task<IIdentityResult> AddRolesToUserAsync(string userId, string[] roleNames);
        Task<IIdentityResult> RemoveRolesFromUserAsync(string userId, string[] roleNames);
        Task<IRegisterResult> RegisterAsync(IRegisterModel model);
        Task<IGetUserResult> GetUserByNameAsync(string userName);
        Task<IGetUserResult> GetUserByPhoneAsync(string userName);
        Task<IGetUsersResult> SearchUsersByNameAsync(string userName);
        Task<IGetUsersResultTextValue> GetUsersByNameAsync(string name);
        Task<IGetRoleResult> GetRoleByIdAsync(string id);
        Task<IGetRoleResult> GetRoleByNameAsync(string name);
        Task<IGetRolesResult> GetAllRolesAsync();
        Task<IIdentityResult> CreateRoleAsync(ICreateRoleModel model);
        Task<IIdentityResult> UpdateRoleAsync(IUpdateRoleModel model);
        Task<IGetUsersResult> GetUsersAsync(List<string> userIds);
        Task<IGetUsersResult> GetUsersAsync();
        Task<IGetUserResult> GetUserAsync(string userId);
        Task<IIdentityResult> UpdateUserAsync(IUpdateUserModel model);
        Task<IIdentityResult> UpdateUserByEmailAsync(IUpdateUserModel model);
        Task<IIdentityResult> ChangePasswordAsync(IChangePasswordModel model);
        Task<IGeneratePasswordResetTokenResult> GeneratePasswordResetTokenAsync(IGeneratePasswordResetTokenModel model);
        Task<IIdentityResult> ResetPasswordByAdminAsync(IResetPasswordByAdminModel model);
        Task<IIdentityResult> ForgetPasswordAsync(IForgetPasswordModel model);
        Task<IGetUserResult> ConfirmEmailAsync(string userId, string code);
        Task<IIdentityResult> ResetPasswordAsync(IResetPasswordModel model);
        Task<IGetUserResult> DeleteUserAsync(Guid userId);
        Task<IGetUserResult> EnableUserAsync(string userId);
        Task<IGetUserResult> DisableUserAsync(string userId);
        Task<IIdentityResult> DeleteRoleAsync(Guid roleId);
    }
}
