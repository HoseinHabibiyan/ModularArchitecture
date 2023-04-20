namespace  ModularArchitecture.Identity
{
    public interface IResetPasswordModel
    {
         public string Username { get; set; }
         public string Password { get; set; }
         public string ConfirmPassword { get; set; }
         public string Token { get; set; }
         string Code { get; set; }
    }
}