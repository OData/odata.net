namespace AbnfParser.CstNodes
{
    using AbnfParser.CstNodes.Core;

    public abstract class ProseVal
    {
        private ProseVal()
        {
        }

        public sealed class x20 : ProseVal
        {
            public x20(x3C lessThan, x20 value, x3E greaterThan)
            {
                LessThan = lessThan;
                Value = value;
                GreaterThan = greaterThan;
            }

            public x3C LessThan { get; }
            public x20 Value { get; }
            public x3E GreaterThan { get; }
        }

        //// TODO finish this
    }
}
