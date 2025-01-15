namespace _GeneratorV5.OldToGeneratedCstConverters
{
    using _GeneratorV5.ManualParsers.Rules;
    using System.Linq;

    public sealed class DecValConverter : AbnfParser.CstNodes.DecVal.Visitor<__Generated.CstNodes.Rules._decⲻval, Root.Void>
    {
        private DecValConverter()
        {
        }

        public static DecValConverter Instance { get; } = new DecValConverter();

        protected internal override __Generated.CstNodes.Rules._decⲻval Accept(AbnfParser.CstNodes.DecVal.DecsOnly node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._decⲻval(
                new __Generated.CstNodes.Inners._ʺx64ʺ(
                    x64Converter.Instance.Convert(node.D)),
                node.Digits.Select(digit =>
                    DigitConverter.Instance.Visit(digit, context)).Convert2(),
                null);
        }

        protected internal override __Generated.CstNodes.Rules._decⲻval Accept(AbnfParser.CstNodes.DecVal.ConcatenatedDecs node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._decⲻval(
                new __Generated.CstNodes.Inners._ʺx64ʺ(
                    x64Converter.Instance.Convert(node.D)),
                node.Digits.Select(digit =>
                    DigitConverter.Instance.Visit(digit, context)).Convert2(),
                new __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._1ЖⲤʺx2Eʺ_1ЖDIGITↃ(
                    node.Inners.Select(inner => 
                        new __Generated.CstNodes.Inners._Ⲥʺx2Eʺ_1ЖDIGITↃ(
                            new __Generated.CstNodes.Inners._ʺx2Eʺ_1ЖDIGIT(
                                new __Generated.CstNodes.Inners._ʺx2Eʺ(
                                    x2EConverter.Instance.Convert(inner.Dot)),
                                inner.Digits.Select(digit =>
                                    DigitConverter.Instance.Visit(digit, context)).Convert2()))).Convert2()));
        }

        protected internal override __Generated.CstNodes.Rules._decⲻval Accept(AbnfParser.CstNodes.DecVal.Range node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._decⲻval(
                new __Generated.CstNodes.Inners._ʺx64ʺ(
                    x64Converter.Instance.Convert(node.D)),
                node.Digits.Select(digit =>
                    DigitConverter.Instance.Visit(digit, context)).Convert2(),
                new __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._Ⲥʺx2Dʺ_1ЖDIGITↃ(
                    new __Generated.CstNodes.Inners._Ⲥʺx2Dʺ_1ЖDIGITↃ(
                        new __Generated.CstNodes.Inners._ʺx2Dʺ_1ЖDIGIT(
                            new __Generated.CstNodes.Inners._ʺx2Dʺ(
                                x2DConverter.Instance.Convert(node.Inners.First().Dash)),
                            node.Inners.First().Digits.Select(digit =>
                                DigitConverter.Instance.Visit(digit, context)).Convert2()))));
        }
    }
}
