namespace  ModularArchitecture.Identity.Core
{
    public interface IForgetPasswordModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string CallbackUrl { get; set; }
    }
}