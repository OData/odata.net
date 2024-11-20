namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;
    
    public sealed class ContextTranslator :
        ConcreteSyntaxTreeNodes.Context.Visitor<
            AbstractSyntaxTreeNodes.Context,
            Void>
    {
        private ContextTranslator()
        {
        }

        public static ContextTranslator Default { get; } = new ContextTranslator();
    }
}
