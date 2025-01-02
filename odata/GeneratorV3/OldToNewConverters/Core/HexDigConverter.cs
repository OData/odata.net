namespace GeneratorV3.OldToNewConverters.Core
{
    using AbnfParser.CstNodes.Core;
    using GeneratorV3.Abnf;
    using Root;

    public sealed class HexDigConverter : AbnfParser.CstNodes.Core.HexDig.Visitor<GeneratorV3.Abnf._HEXDIG, Root.Void>
    {
        private HexDigConverter()
        {
        }

        public static HexDigConverter Instance { get; } = new HexDigConverter();

        protected internal override _HEXDIG Accept(HexDig.Digit node, Void context)
        {
            return new _HEXDIG._DIGIT(
                DigitConverter.Instance.Visit(node.Value, context));
        }

        protected internal override _HEXDIG Accept(HexDig.A node, Void context)
        {
            return new _HEXDIG._ʺx41ʺ(
                new Inners._ʺx41ʺ(
                    Inners._x41.Instance));
        }

        protected internal override _HEXDIG Accept(HexDig.B node, Void context)
        {
            return new _HEXDIG._ʺx42ʺ(
                new Inners._ʺx42ʺ(
                    Inners._x42.Instance));
        }

        protected internal override _HEXDIG Accept(HexDig.C node, Void context)
        {
            return new _HEXDIG._ʺx43ʺ(
                new Inners._ʺx43ʺ(
                    Inners._x43.Instance));
        }

        protected internal override _HEXDIG Accept(HexDig.D node, Void context)
        {
            return new _HEXDIG._ʺx44ʺ(
                new Inners._ʺx44ʺ(
                    Inners._x44.Instance));
        }

        protected internal override _HEXDIG Accept(HexDig.E node, Void context)
        {
            return new _HEXDIG._ʺx45ʺ(
                new Inners._ʺx45ʺ(
                    Inners._x45.Instance));
        }

        protected internal override _HEXDIG Accept(HexDig.F node, Void context)
        {
            return new _HEXDIG._ʺx46ʺ(
                new Inners._ʺx46ʺ(
                    Inners._x46.Instance));
        }
    }
}
