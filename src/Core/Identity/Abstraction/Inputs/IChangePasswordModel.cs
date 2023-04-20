namespace ModularArchitecture.Identity.Abstraction.Inputs
{
    public interface IChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}