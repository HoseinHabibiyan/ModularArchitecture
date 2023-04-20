namespace   ModularArchitecture.Identity.Core
{
    public interface IIdentityResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}