namespace AbnfParser.CstNodes.Core
{
    public sealed class Dquote
    {
        public Dquote(x22 value)
        {
            Value = value;
        }

        public x22 Value { get; }
    }
}
