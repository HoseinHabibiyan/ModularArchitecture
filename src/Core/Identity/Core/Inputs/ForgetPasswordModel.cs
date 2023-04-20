using ModularArchitecture.Identity.Abstraction.Inputs;

namespace ModularArchitecture.Identity.Core.Inputs
{
    public class ForgetPasswordModel : IForgetPasswordModel
    {
        [Obsolete("Use Username")]
        public string Email { get; set; }
        public string Username { get; set; }
        public string CallbackUrl { get; set; }
    }
}
