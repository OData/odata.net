namespace _GeneratorV4.OldToV4Converters
{
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;
    using Root;

    public sealed class DecValConverter : AbnfParser.CstNodes.DecVal.Visitor<_decⲻval, Root.Void>
    {
        private DecValConverter()
        {
        }

        public static DecValConverter Instance { get; } = new DecValConverter();

        protected internal override _decⲻval Accept(DecVal.DecsOnly node, Void context)
        {
            return new _decⲻval(
                new Inners._ʺx64ʺ(
                    x64Converter.Instance.Convert(node.D)),
                node.Digits.Select(digit =>
                    DigitConverter.Instance.Visit(digit, context)),
                null);
        }

        protected internal override _decⲻval Accept(DecVal.ConcatenatedDecs node, Void context)
        {
            return new _decⲻval(
                new Inners._ʺx64ʺ(
                    x64Converter.Instance.Convert(node.D)),
                node.Digits.Select(digit =>
                    DigitConverter.Instance.Visit(digit, context)),
                new Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._1ЖⲤʺx2Eʺ_1ЖDIGITↃ(
                    node.Inners.Select(inner => 
                        new Inners._Ⲥʺx2Eʺ_1ЖDIGITↃ(
                            new Inners._ʺx2Eʺ_1ЖDIGIT(
                                new Inners._ʺx2Eʺ(
                                    x2EConverter.Instance.Convert(inner.Dot)),
                                inner.Digits.Select(digit =>
                                    DigitConverter.Instance.Visit(digit, context)))))));
        }

        protected internal override _decⲻval Accept(DecVal.Range node, Void context)
        {
            return new _decⲻval(
                new Inners._ʺx64ʺ(
                    x64Converter.Instance.Convert(node.D)),
                node.Digits.Select(digit =>
                    DigitConverter.Instance.Visit(digit, context)),
                new Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._Ⲥʺx2Dʺ_1ЖDIGITↃ(
                    new Inners._Ⲥʺx2Dʺ_1ЖDIGITↃ(
                        new Inners._ʺx2Dʺ_1ЖDIGIT(
                            new Inners._ʺx2Dʺ(
                                x2DConverter.Instance.Convert(node.Inners.First().Dash)),
                            node.Inners.First().Digits.Select(digit =>
                                DigitConverter.Instance.Visit(digit, context))))));
        }
    }
}
