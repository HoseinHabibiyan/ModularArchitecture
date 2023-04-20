namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface IGetUsersResultTextValue : IIdentityResult
    {
        List<ITextValue> Users { get; set; }
    }
}