using ModularArchitecture.Identity.Abstraction.Inputs;

namespace ModularArchitecture.Identity.Core.Inputs
{
    public class ResetPasswordByAdminModel : IResetPasswordByAdminModel
    {
        [Obsolete("Use Username")]
        public string Email { get; set; }
        public string Username { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
