namespace AbnfParser.CstNodes.Core
{
    public abstract class Bit
    {
        private Bit()
        {
        }

        public sealed class Zero : Bit
        {
            public Zero(x30 value)
            {
                Value = value;
            }

            public x30 Value { get; }
        }

        public sealed class One : Bit
        {
            public One(x31 value)
            {
                Value = value;
            }

            public x31 Value { get; }
        }
    }
}
