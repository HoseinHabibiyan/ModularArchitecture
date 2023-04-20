namespace  ModularArchitecture.Identity
{
    public interface IGeneratePasswordResetTokenModel
    {
        public string UserName { get; set; }
    }
}