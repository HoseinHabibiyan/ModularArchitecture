using ModularArchitecture.Identity.Abstraction.Results;

namespace ModularArchitecture.Identity.Core.Results
{
    public class IdentityResult : IIdentityResult
    {
        public IdentityResult()
        {

        }

        public IdentityResult(string message)
        {
            Message = message;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
    }
}