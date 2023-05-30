namespace ModularArchitecture.Identity.Abstraction.Results
{
    public interface IAuthToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}