namespace _GeneratorV5.OldToGeneratedCstConverters
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
            return new __Generated.CstNodes.Rules._repeat._Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ(
                new __Generated.CstNodes.Inners._Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ(
                    new __Generated.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡(
                        node.Digits.Select(digit =>
                            DigitConverter.Instance.Visit(digit, context)),
                        null)));
        }

        protected internal override __Generated.CstNodes.Rules._repeat Accept(AbnfParser.CstNodes.Repeat.Range node, Root.Void context)
        {
            if (node.PrefixDigits.Any())
            {
                return new __Generated.CstNodes.Rules._repeat._Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ(
                    new __Generated.CstNodes.Inners._Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ(
                        new __Generated.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡(
                            node.PrefixDigits.Select(
                                digit => DigitConverter.Instance.Visit(digit, context)),
                            new __Generated.CstNodes.Inners._ʺx2Aʺ_ЖDIGIT(
                                new __Generated.CstNodes.Inners._ʺx2Aʺ(
                                    x2AConverter.Instance.Convert(node.Asterisk)),
                                node.SuffixDigits.Select(
                                    digit => DigitConverter.Instance.Visit(digit, context))))));
            }
            else
            {
                return new __Generated.CstNodes.Rules._repeat._Ⲥʺx2Aʺ_ЖDIGITↃ(
                    new __Generated.CstNodes.Inners._Ⲥʺx2Aʺ_ЖDIGITↃ(
                        new __Generated.CstNodes.Inners._ʺx2Aʺ_ЖDIGIT(
                                new __Generated.CstNodes.Inners._ʺx2Aʺ(
                                    x2AConverter.Instance.Convert(node.Asterisk)),
                                node.SuffixDigits.Select(
                                    digit => DigitConverter.Instance.Visit(digit, context)))));
            }
        }
    }
}
