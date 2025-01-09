namespace _GeneratorV4.OldToGeneratedCstConverters
{
    public sealed class NumValConverter : AbnfParser.CstNodes.NumVal.Visitor<__Generated.CstNodes.Rules._numⲻval, Root.Void>
    {
        private NumValConverter()
        {
        }

        public static NumValConverter Instance { get; } = new NumValConverter();

        protected internal override __Generated.CstNodes.Rules._numⲻval Accept(AbnfParser.CstNodes.NumVal.BinVal node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._numⲻval(
                new __Generated.CstNodes.Inners._ʺx25ʺ(
                    x25Converter.Instance.Convert(node.Percent)),
                new __Generated.CstNodes.Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ(
                    new __Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._binⲻval(
                        BinValConverter.Instance.Visit(node.Value, context))));
        }

        protected internal override __Generated.CstNodes.Rules._numⲻval Accept(AbnfParser.CstNodes.NumVal.DecVal node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._numⲻval(
                new __Generated.CstNodes.Inners._ʺx25ʺ(
                    x25Converter.Instance.Convert(node.Percent)),
                new __Generated.CstNodes.Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ(
                    new __Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._decⲻval(
                        DecValConverter.Instance.Visit(node.Value, context))));
        }

        protected internal override __Generated.CstNodes.Rules._numⲻval Accept(AbnfParser.CstNodes.NumVal.HexVal node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._numⲻval(
                new __Generated.CstNodes.Inners._ʺx25ʺ(
                    x25Converter.Instance.Convert(node.Percent)),
                new __Generated.CstNodes.Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ(
                    new __Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._hexⲻval(
                        HexValConverter.Instance.Visit(node.Value, context))));
        }
    }
}
