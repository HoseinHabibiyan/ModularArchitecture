namespace  ModularArchitecture.Identity
{
    public interface IConfirmEmailModel
    {
        string UserId { get; set; }
        string Token { get; set; }
    }
}