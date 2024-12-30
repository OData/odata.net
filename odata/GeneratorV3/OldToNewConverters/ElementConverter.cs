namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using Root;

    public sealed class ElementConverter : AbnfParser.CstNodes.Element.Visitor<GeneratorV3.Abnf._element, Root.Void>
    {
        private ElementConverter()
        {
        }

        public static ElementConverter Instance { get; } = new ElementConverter();

        protected internal override _element Accept(Element.RuleName node, Void context)
        {
            return new _element._rulename(
                RuleNameConverter.Instance.Convert(node.Value));
        }

        protected internal override _element Accept(Element.Group node, Void context)
        {
            return new _element._group(
                GroupConverter.Instance.Convert(node.Value));
        }

        protected internal override _element Accept(Element.Option node, Void context)
        {
            return new _element._option(
                OptionConverter.Instance.Convert(node.Value));
        }

        protected internal override _element Accept(Element.CharVal node, Void context)
        {
            return new _element._charⲻval(
                CharValConverter.Instance.Convert(node.Value));
        }

        protected internal override _element Accept(Element.NumVal node, Void context)
        {
            return new _element._numⲻval(
                NumValConverter.Instance.Visit(node.Value, context));
        }

        protected internal override _element Accept(Element.ProseVal node, Void context)
        {
            throw new System.Exception("TODO");
        }
    }
}
