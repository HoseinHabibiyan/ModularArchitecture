
namespace  ModularArchitecture.Identity.Core
{
    public class ConfirmEmailSetPasswordAddToRolesModel : IConfirmEmailSetPasswordAddToRolesModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string[] Roles { get; set; }
    }
}
