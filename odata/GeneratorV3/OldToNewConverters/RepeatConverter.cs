namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using GeneratorV3.OldToNewConverters.Core;
    using Root;

    public sealed class RepeatConverter : AbnfParser.CstNodes.Repeat.Visitor<GeneratorV3.Abnf._repeat, Root.Void>
    {
        private RepeatConverter()
        {
        }

        public static RepeatConverter Instance { get; } = new RepeatConverter();

        protected internal override _repeat Accept(Repeat.Count node, Void context)
        {
            return new _repeat._1ЖDIGIT(
                node.Digits.Select(digit =>
                    DigitConverter.Instance.Visit(digit, context)));
        }

        protected internal override _repeat Accept(Repeat.Range node, Void context)
        {
            return new _repeat._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ(
                new Inners._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ(
                    new Inners._ЖDIGIT_ʺx2Aʺ_ЖDIGIT(
                        node.PrefixDigits.Select(digit =>
                            DigitConverter.Instance.Visit(digit, context)),
                        new Inners._ʺx2Aʺ(
                            x2AConverter.Instance.Convert(node.Asterisk)),
                        node.SuffixDigits.Select(digit =>
                            DigitConverter.Instance.Visit(digit, context)))));
        }
    }
}
