namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface IIdentityResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}