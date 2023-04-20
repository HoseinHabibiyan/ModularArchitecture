using ModularArchitecture.Identity.Core;

namespace ModularArchitecture.Identity
{
    public interface IGetUsersResultTextValue : IIdentityResult
    {
        List<ITextValue> Users { get; set; }
    }
}