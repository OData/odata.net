namespace _GeneratorV4.OldToGeneratedCstConverters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class ElementConverter : AbnfParser.CstNodes.Element.Visitor<__Generated.CstNodes.Rules._element, Root.Void>
    {
        private ElementConverter()
        {
        }

        public static ElementConverter Instance { get; } = new ElementConverter();

        protected internal override __Generated.CstNodes.Rules._element Accept(AbnfParser.CstNodes.Element.RuleName node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._element._rulename(
                RuleNameConverter.Instance.Convert(node.Value));
        }

        protected internal override __Generated.CstNodes.Rules._element Accept(AbnfParser.CstNodes.Element.Group node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._element._group(
                GroupConverter.Instance.Convert(node.Value));
        }

        protected internal override __Generated.CstNodes.Rules._element Accept(AbnfParser.CstNodes.Element.Option node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._element._option(
                OptionConverter.Instance.Convert(node.Value));
        }

        protected internal override __Generated.CstNodes.Rules._element Accept(AbnfParser.CstNodes.Element.CharVal node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._element._charⲻval(
                CharValConverter.Instance.Convert(node.Value));
        }

        protected internal override __Generated.CstNodes.Rules._element Accept(AbnfParser.CstNodes.Element.NumVal node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._element._numⲻval(
                NumValConverter.Instance.Visit(node.Value, context));
        }

        protected internal override __Generated.CstNodes.Rules._element Accept(AbnfParser.CstNodes.Element.ProseVal node, Root.Void context)
        {
            throw new System.Exception("TODO");
        }
    }
}
