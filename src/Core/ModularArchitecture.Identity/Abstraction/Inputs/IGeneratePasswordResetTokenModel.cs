namespace ModularArchitecture.Identity.Abstraction.Inputs
{
    public interface IGeneratePasswordResetTokenModel
    {
        public string UserName { get; set; }
    }
}