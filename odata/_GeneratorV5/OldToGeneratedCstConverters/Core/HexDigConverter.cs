namespace _GeneratorV4.OldToGeneratedCstConverters
{
    public sealed class HexDigConverter : AbnfParser.CstNodes.Core.HexDig.Visitor<__Generated.CstNodes.Rules._HEXDIG, Root.Void>
    {
        private HexDigConverter()
        {
        }

        public static HexDigConverter Instance { get; } = new HexDigConverter();

        protected internal override __Generated.CstNodes.Rules._HEXDIG Accept(AbnfParser.CstNodes.Core.HexDig.Digit node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._HEXDIG._DIGIT(
                DigitConverter.Instance.Visit(node.Value, context));
        }

        protected internal override __Generated.CstNodes.Rules._HEXDIG Accept(AbnfParser.CstNodes.Core.HexDig.A node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._HEXDIG._ʺx41ʺ(
                new __Generated.CstNodes.Inners._ʺx41ʺ(
                    __Generated.CstNodes.Inners._x41.Instance));
        }

        protected internal override __Generated.CstNodes.Rules._HEXDIG Accept(AbnfParser.CstNodes.Core.HexDig.B node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._HEXDIG._ʺx42ʺ(
                new __Generated.CstNodes.Inners._ʺx42ʺ(
                    __Generated.CstNodes.Inners._x42.Instance));
        }

        protected internal override __Generated.CstNodes.Rules._HEXDIG Accept(AbnfParser.CstNodes.Core.HexDig.C node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._HEXDIG._ʺx43ʺ(
                new __Generated.CstNodes.Inners._ʺx43ʺ(
                    __Generated.CstNodes.Inners._x43.Instance));
        }

        protected internal override __Generated.CstNodes.Rules._HEXDIG Accept(AbnfParser.CstNodes.Core.HexDig.D node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._HEXDIG._ʺx44ʺ(
                new __Generated.CstNodes.Inners._ʺx44ʺ(
                    __Generated.CstNodes.Inners._x44.Instance));
        }

        protected internal override __Generated.CstNodes.Rules._HEXDIG Accept(AbnfParser.CstNodes.Core.HexDig.E node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._HEXDIG._ʺx45ʺ(
                new __Generated.CstNodes.Inners._ʺx45ʺ(
                    __Generated.CstNodes.Inners._x45.Instance));
        }

        protected internal override __Generated.CstNodes.Rules._HEXDIG Accept(AbnfParser.CstNodes.Core.HexDig.F node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._HEXDIG._ʺx46ʺ(
                new __Generated.CstNodes.Inners._ʺx46ʺ(
                    __Generated.CstNodes.Inners._x46.Instance));
        }
    }
}
