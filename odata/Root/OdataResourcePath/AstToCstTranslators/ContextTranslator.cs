namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;
    
    public sealed class ContextTranslator :
        AbstractSyntaxTreeNodes.Context.Visitor<
            ConcreteSyntaxTreeNodes.Context,
            Void>
    {
        private ContextTranslator()
        {
        }

        public static ContextTranslator Default { get; } = new ContextTranslator();
    }
}
