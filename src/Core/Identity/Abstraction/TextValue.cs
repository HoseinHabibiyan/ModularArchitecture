namespace ModularArchitecture.Identity.Abstraction
{
    public class TextValue : ITextValue
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}