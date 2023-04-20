using ModularArchitecture.Identity.Abstraction.Inputs;

namespace ModularArchitecture.Identity.Core.Inputs
{
    public class GeneratePasswordResetTokenModel : IGeneratePasswordResetTokenModel
    {
        public string UserName { get; set; }
    }
}
