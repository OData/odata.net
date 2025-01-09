namespace _GeneratorV4.OldToGeneratedCstConverters
{
    using System.Linq;

    public sealed class RepeatConverter : AbnfParser.CstNodes.Repeat.Visitor<__Generated.CstNodes.Rules._repeat, Root.Void>
    {
        private RepeatConverter()
        {
        }

        public static RepeatConverter Instance { get; } = new RepeatConverter();

        protected internal override __Generated.CstNodes.Rules._repeat Accept(AbnfParser.CstNodes.Repeat.Count node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._repeat._1ЖDIGIT(
                node.Digits.Select(digit =>
                    DigitConverter.Instance.Visit(digit, context)));
        }

        protected internal override __Generated.CstNodes.Rules._repeat Accept(AbnfParser.CstNodes.Repeat.Range node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._repeat._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ(
                new __Generated.CstNodes.Inners._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ(
                    new __Generated.CstNodes.Inners._ЖDIGIT_ʺx2Aʺ_ЖDIGIT(
                        node.PrefixDigits.Select(digit =>
                            DigitConverter.Instance.Visit(digit, context)),
                        new __Generated.CstNodes.Inners._ʺx2Aʺ(
                            x2AConverter.Instance.Convert(node.Asterisk)),
                        node.SuffixDigits.Select(digit =>
                            DigitConverter.Instance.Visit(digit, context)))));
        }
    }
}
