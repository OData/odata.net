namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class DigitConverter : AbnfParser.CstNodes.Core.Digit.Visitor<_DIGIT, Root.Void>
    {
        private DigitConverter()
        {
        }

        public static DigitConverter Instance { get; } = new DigitConverter();

        protected internal override _DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x30 node, Root.Void context)
        {
            return new _DIGIT(
                new Inners._Ⰳx30ⲻ39._30(
                    Inners._3.Instance,
                    Inners._0.Instance));
        }

        protected internal override _DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x31 node, Root.Void context)
        {
            return new _DIGIT(
                new Inners._Ⰳx30ⲻ39._31(
                    Inners._3.Instance,
                    Inners._1.Instance));
        }

        protected internal override _DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x32 node, Root.Void context)
        {
            return new _DIGIT(
                new Inners._Ⰳx30ⲻ39._32(
                    Inners._3.Instance,
                    Inners._2.Instance));
        }

        protected internal override _DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x33 node, Root.Void context)
        {
            return new _DIGIT(
                new Inners._Ⰳx30ⲻ39._33(
                    Inners._3.Instance,
                    Inners._3.Instance));
        }

        protected internal override _DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x34 node, Root.Void context)
        {
            return new _DIGIT(
                new Inners._Ⰳx30ⲻ39._34(
                    Inners._3.Instance,
                    Inners._4.Instance));
        }

        protected internal override _DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x35 node, Root.Void context)
        {
            return new _DIGIT(
                new Inners._Ⰳx30ⲻ39._35(
                    Inners._3.Instance,
                    Inners._5.Instance));
        }

        protected internal override _DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x36 node, Root.Void context)
        {
            return new _DIGIT(
                new Inners._Ⰳx30ⲻ39._36(
                    Inners._3.Instance,
                    Inners._6.Instance));
        }

        protected internal override _DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x37 node, Root.Void context)
        {
            return new _DIGIT(
                new Inners._Ⰳx30ⲻ39._37(
                    Inners._3.Instance,
                    Inners._7.Instance));
        }

        protected internal override _DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x38 node, Root.Void context)
        {
            return new _DIGIT(
                new Inners._Ⰳx30ⲻ39._38(
                    Inners._3.Instance,
                    Inners._8.Instance));
        }

        protected internal override _DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x39 node, Root.Void context)
        {
            return new _DIGIT(
                new Inners._Ⰳx30ⲻ39._39(
                    Inners._3.Instance,
                    Inners._9.Instance));
        }
    }
}
