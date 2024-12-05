namespace AbnfParser.CstNodes
{
    public abstract class NumVal
    {
        private NumVal()
        {
        }

        public sealed class BinVal : NumVal
        {
            public BinVal(CstNodes.BinVal value)
            {
                Value = value;
            }

            public CstNodes.BinVal Value { get; }
        }

        public sealed class DecVal : NumVal
        {
            public DecVal(CstNodes.DecVal value)
            {
                Value = value;
            }

            public CstNodes.DecVal Value { get; }
        }

        public sealed class HexVal : NumVal
        {
            public HexVal(CstNodes.HexVal value)
            {
                Value = value;
            }

            public CstNodes.HexVal Value { get; }
        }
    }
}
