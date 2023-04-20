namespace ModularArchitecture.Identity.Core
{ 
    public class ForgetPasswordModel : IForgetPasswordModel
    {
        [Obsolete("Use Username")]
        public string Email { get; set; }
        public string Username { get; set; }
        public string CallbackUrl { get; set; }
    }
}
