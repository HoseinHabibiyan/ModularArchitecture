namespace   ModularArchitecture.Identity.Core
{
    public interface IAuthToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}