namespace ModularArchitecture.Identity.Abstraction.Inputs
{
    public interface IConfirmEmailSetPasswordAddToRolesModel
    {
        string Email { get; set; }
        string Token { get; set; }
        string Password { get; set; }
        public string ConfirmPassword { get; set; }
        string[] Roles { get; set; }
    }
}