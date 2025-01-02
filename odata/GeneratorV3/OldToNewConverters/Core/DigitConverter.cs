namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class DigitConverter : AbnfParser.CstNodes.Core.Digit.Visitor<GeneratorV3.Abnf._DIGIT, Root.Void>
    {
        private DigitConverter()
        {
        }

        public static DigitConverter Instance { get; } = new DigitConverter();

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x30 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentx30ⲻ39._30(
                    Abnf.Inners._3.Instance,
                    Abnf.Inners._0.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x31 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentx30ⲻ39._31(
                    Abnf.Inners._3.Instance,
                    Abnf.Inners._1.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x32 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentx30ⲻ39._32(
                    Abnf.Inners._3.Instance,
                    Abnf.Inners._2.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x33 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentx30ⲻ39._33(
                    Abnf.Inners._3.Instance,
                    Abnf.Inners._3.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x34 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentx30ⲻ39._34(
                    Abnf.Inners._3.Instance,
                    Abnf.Inners._4.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x35 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentx30ⲻ39._35(
                    Abnf.Inners._3.Instance,
                    Abnf.Inners._5.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x36 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentx30ⲻ39._36(
                    Abnf.Inners._3.Instance,
                    Abnf.Inners._6.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x37 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentx30ⲻ39._37(
                    Abnf.Inners._3.Instance,
                    Abnf.Inners._7.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x38 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentx30ⲻ39._38(
                    Abnf.Inners._3.Instance,
                    Abnf.Inners._8.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x39 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentx30ⲻ39._39(
                    Abnf.Inners._3.Instance,
                    Abnf.Inners._9.Instance));
        }
    }
}
