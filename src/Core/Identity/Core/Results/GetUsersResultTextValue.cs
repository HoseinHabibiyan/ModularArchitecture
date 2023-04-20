using ModularArchitecture.Identity.Core.Result;

namespace  ModularArchitecture.Identity.Core
{
    public class GetUsersResultTextValue : IdentityResult, IGetUsersResultTextValue
    {
        public List<ITextValue> Users { get; set; }
    }
}