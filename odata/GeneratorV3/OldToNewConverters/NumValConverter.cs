namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using GeneratorV3.OldToNewConverters.Core;
    using Root;

    public sealed class NumValConverter : AbnfParser.CstNodes.NumVal.Visitor<GeneratorV3.Abnf._numⲻval, Root.Void>
    {
        private NumValConverter()
        {
        }

        public static NumValConverter Instance { get; } = new NumValConverter();

        protected internal override _numⲻval Accept(NumVal.BinVal node, Void context)
        {
            return new _numⲻval(
                new Inners._doublequotex25doublequote(
                    x25Converter.Instance.Convert(node.Percent)),
                new Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ(
                    new Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._binⲻval(
                        BinValConverter.Instance.Visit(node.Value, context))));
        }

        protected internal override _numⲻval Accept(NumVal.DecVal node, Void context)
        {
            return new _numⲻval(
                new Inners._doublequotex25doublequote(
                    x25Converter.Instance.Convert(node.Percent)),
                new Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ(
                    new Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._decⲻval(
                        DecValConverter.Instance.Visit(node.Value, context))));
        }

        protected internal override _numⲻval Accept(NumVal.HexVal node, Void context)
        {
            return new _numⲻval(
                new Inners._doublequotex25doublequote(
                    x25Converter.Instance.Convert(node.Percent)),
                new Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ(
                    new Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._hexⲻval(
                        HexValConverter.Instance.Visit(node.Value, context))));
        }
    }
}
