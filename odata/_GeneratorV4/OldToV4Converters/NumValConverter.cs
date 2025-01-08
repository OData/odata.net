namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;
    using Root;

    public sealed class NumValConverter : AbnfParser.CstNodes.NumVal.Visitor<_numⲻval, Root.Void>
    {
        private NumValConverter()
        {
        }

        public static NumValConverter Instance { get; } = new NumValConverter();

        protected internal override _numⲻval Accept(NumVal.BinVal node, Void context)
        {
            return new _numⲻval(
                new Inners._ʺx25ʺ(
                    x25Converter.Instance.Convert(node.Percent)),
                new Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ(
                    new Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._binⲻval(
                        BinValConverter.Instance.Visit(node.Value, context))));
        }

        protected internal override _numⲻval Accept(NumVal.DecVal node, Void context)
        {
            return new _numⲻval(
                new Inners._ʺx25ʺ(
                    x25Converter.Instance.Convert(node.Percent)),
                new Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ(
                    new Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._decⲻval(
                        DecValConverter.Instance.Visit(node.Value, context))));
        }

        protected internal override _numⲻval Accept(NumVal.HexVal node, Void context)
        {
            return new _numⲻval(
                new Inners._ʺx25ʺ(
                    x25Converter.Instance.Convert(node.Percent)),
                new Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ(
                    new Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._hexⲻval(
                        HexValConverter.Instance.Visit(node.Value, context))));
        }
    }
}
