
namespace  ModularArchitecture.Identity.Core
{
    public class ChangePasswordModel : IChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
