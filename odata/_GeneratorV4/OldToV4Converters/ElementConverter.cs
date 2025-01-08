namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;
    using Root;

    public sealed class ElementConverter : AbnfParser.CstNodes.Element.Visitor<_element, Root.Void>
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
