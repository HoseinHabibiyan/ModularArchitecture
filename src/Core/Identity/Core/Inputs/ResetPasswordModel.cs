using    ModularArchitecture.Identity.Core;

namespace  ModularArchitecture.Identity.Core
{
    public class ResetPasswordModel : IResetPasswordModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
        public string Code { get; set; }
    }
}
