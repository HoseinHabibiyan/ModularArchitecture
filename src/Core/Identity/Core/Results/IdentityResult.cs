
namespace  ModularArchitecture.Identity.Core.Result
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