namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using GeneratorV3.OldToNewConverters.Core;
    using Root;

    public sealed class DecValConverter : AbnfParser.CstNodes.DecVal.Visitor<GeneratorV3.Abnf._decⲻval, Root.Void>
    {
        private DecValConverter()
        {
        }

        public static DecValConverter Instance { get; } = new DecValConverter();

        protected internal override _decⲻval Accept(DecVal.DecsOnly node, Void context)
        {
            return new _decⲻval(
                new Inners._doublequotex64doublequote(
                    x64Converter.Instance.Convert(node.D)),
                node.Digits.Select(digit =>
                    DigitConverter.Instance.Visit(digit, context)),
                null);
        }

        protected internal override _decⲻval Accept(DecVal.ConcatenatedDecs node, Void context)
        {
            return new _decⲻval(
                new Inners._doublequotex64doublequote(
                    x64Converter.Instance.Convert(node.D)),
                node.Digits.Select(digit =>
                    DigitConverter.Instance.Visit(digit, context)),
                new Inners._ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ._ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃ(
                    node.Inners.Select(inner => 
                        new Inners._opendoublequotex2Edoublequote_ONEasteriskDIGITↃ(
                            new Inners._doublequotex2Edoublequote_ONEasteriskDIGIT(
                                new Inners._doublequotex2Edoublequote(
                                    x2EConverter.Instance.Convert(inner.Dot)),
                                inner.Digits.Select(digit =>
                                    DigitConverter.Instance.Visit(digit, context)))))));
        }

        protected internal override _decⲻval Accept(DecVal.Range node, Void context)
        {
            return new _decⲻval(
                new Inners._doublequotex64doublequote(
                    x64Converter.Instance.Convert(node.D)),
                node.Digits.Select(digit =>
                    DigitConverter.Instance.Visit(digit, context)),
                new Inners._ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ._opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ(
                    new Inners._opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ(
                        new Inners._doublequotex2Ddoublequote_ONEasteriskDIGIT(
                            new Inners._doublequotex2Ddoublequote(
                                x2DConverter.Instance.Convert(node.Inners.First().Dash)),
                            node.Inners.First().Digits.Select(digit =>
                                DigitConverter.Instance.Visit(digit, context))))));
        }
    }
}
