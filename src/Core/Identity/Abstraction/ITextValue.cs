namespace ModularArchitecture.Identity
{
    public interface ITextValue
    {
        string Text { get; set; }
        string Value { get; set; }
        bool Selected { get; set; }
    }
}