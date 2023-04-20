namespace ModularArchitecture.Identity.Abstraction.Inputs
{
    public interface IConfirmEmailModel
    {
        string UserId { get; set; }
        string Token { get; set; }
    }
}