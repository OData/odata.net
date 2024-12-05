namespace AbnfParser.CstNodes.Core
{
    public abstract class HexDig
    {
        private HexDig()
        {
        }

        public sealed class Digit : HexDig
        {
            public Digit(Core.Digit value)
            {
                Value = value;
            }

            public Core.Digit Value { get; }
        }

        public sealed class A : HexDig
        {
            public A(x41 value)
            {
                Value = value;
            }

            public x41 Value { get; }
        }

        public sealed class B : HexDig
        {
            public B(x42 value)
            {
                Value = value;
            }

            public x42 Value { get; }
        }

        public sealed class C : HexDig
        {
            public C(x43 value)
            {
                Value = value;
            }

            public x43 Value { get; }
        }

        public sealed class D : HexDig
        {
            public D(x44 value)
            {
                Value = value;
            }

            public x44 Value { get; }
        }

        public sealed class E : HexDig
        {
            public E(x45 value)
            {
                Value = value;
            }

            public x45 Value { get; }
        }

        public sealed class F : HexDig
        {
            public F(x46 value)
            {
                Value = value;
            }

            public x46 Value { get; }
        }
    }
}
