using ModularArchitecture.Identity.Abstraction;
using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class GetUsersResultTextValue : IdentityResult, IGetUsersResultTextValue
    {
        public List<ITextValue> Users { get; set; }
    }
}