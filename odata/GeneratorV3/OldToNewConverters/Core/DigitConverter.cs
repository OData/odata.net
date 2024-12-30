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
                new Abnf.Inners._percentxTHREEZEROⲻTHREENINE._THREEZERO(
                    Abnf.Inners._THREE.Instance,
                    Abnf.Inners._ZERO.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x31 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentxTHREEZEROⲻTHREENINE._THREEONE(
                    Abnf.Inners._THREE.Instance,
                    Abnf.Inners._ONE.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x32 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentxTHREEZEROⲻTHREENINE._THREETWO(
                    Abnf.Inners._THREE.Instance,
                    Abnf.Inners._TWO.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x33 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentxTHREEZEROⲻTHREENINE._THREETHREE(
                    Abnf.Inners._THREE.Instance,
                    Abnf.Inners._THREE.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x34 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentxTHREEZEROⲻTHREENINE._THREEFOUR(
                    Abnf.Inners._THREE.Instance,
                    Abnf.Inners._FOUR.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x35 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentxTHREEZEROⲻTHREENINE._THREEFIVE(
                    Abnf.Inners._THREE.Instance,
                    Abnf.Inners._FIVE.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x36 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentxTHREEZEROⲻTHREENINE._THREESIX(
                    Abnf.Inners._THREE.Instance,
                    Abnf.Inners._SIX.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x37 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentxTHREEZEROⲻTHREENINE._THREESEVEN(
                    Abnf.Inners._THREE.Instance,
                    Abnf.Inners._SEVEN.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x38 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentxTHREEZEROⲻTHREENINE._THREEEIGHT(
                    Abnf.Inners._THREE.Instance,
                    Abnf.Inners._EIGHT.Instance));
        }

        protected internal override GeneratorV3.Abnf._DIGIT Accept(AbnfParser.CstNodes.Core.Digit.x39 node, Root.Void context)
        {
            return new Abnf._DIGIT(
                new Abnf.Inners._percentxTHREEZEROⲻTHREENINE._THREENINE(
                    Abnf.Inners._THREE.Instance,
                    Abnf.Inners._NINE.Instance));
        }
    }
}
