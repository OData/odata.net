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
            return new _HEXDIG._doublequotex41doublequote(
                new Inners._doublequotex41doublequote(
                    Inners._x41.Instance));
        }

        protected internal override _HEXDIG Accept(HexDig.B node, Void context)
        {
            return new _HEXDIG._doublequotex42doublequote(
                new Inners._doublequotex42doublequote(
                    Inners._x42.Instance));
        }

        protected internal override _HEXDIG Accept(HexDig.C node, Void context)
        {
            return new _HEXDIG._doublequotex43doublequote(
                new Inners._doublequotex43doublequote(
                    Inners._x43.Instance));
        }

        protected internal override _HEXDIG Accept(HexDig.D node, Void context)
        {
            return new _HEXDIG._doublequotex44doublequote(
                new Inners._doublequotex44doublequote(
                    Inners._x44.Instance));
        }

        protected internal override _HEXDIG Accept(HexDig.E node, Void context)
        {
            return new _HEXDIG._doublequotex45doublequote(
                new Inners._doublequotex45doublequote(
                    Inners._x45.Instance));
        }

        protected internal override _HEXDIG Accept(HexDig.F node, Void context)
        {
            return new _HEXDIG._doublequotex46doublequote(
                new Inners._doublequotex46doublequote(
                    Inners._x46.Instance));
        }
    }
}
