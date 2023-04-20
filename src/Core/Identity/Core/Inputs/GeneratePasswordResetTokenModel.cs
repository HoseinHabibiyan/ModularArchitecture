using    ModularArchitecture.Identity.Core;

namespace  ModularArchitecture.Identity.Core
{
    public class GeneratePasswordResetTokenModel : IGeneratePasswordResetTokenModel
    {
        public string UserName { get; set; }
    }
}
