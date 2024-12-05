using AbnfParser.CstNodes.Core;

namespace AbnfParser.CstNodes
{
    public abstract class NumVal
    {
        private NumVal()
        {
        }

        public sealed class BinVal : NumVal
        {
            public BinVal(x25 percent, CstNodes.BinVal value)
            {
                Percent = percent;
                Value = value;
            }

            public x25 Percent { get; }
            public CstNodes.BinVal Value { get; }
        }

        public sealed class DecVal : NumVal
        {
            public DecVal(x25 percent, CstNodes.DecVal value)
            {
                Percent = percent;
                Value = value;
            }

            public x25 Percent { get; }
            public CstNodes.DecVal Value { get; }
        }

        public sealed class HexVal : NumVal
        {
            public HexVal(x25 percent, CstNodes.HexVal value)
            {
                Percent = percent;
                Value = value;
            }

            public x25 Percent { get; }
            public CstNodes.HexVal Value { get; }
        }
    }
}
