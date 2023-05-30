namespace ModularArchitecture.Identity.Abstraction
{
    public interface ITextValue
    {
        string Text { get; set; }
        string Value { get; set; }
        bool Selected { get; set; }
    }
}