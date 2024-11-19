namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;
    
    public sealed class ContextTranslator :
        ConcreteSyntaxTree.Context.Visitor<
            AbstractSyntaxTree.Context,
            Void>
    {
        private ContextTranslator()
        {
        }

        public static ContextTranslator Default { get; } = new ContextTranslator();
    }
}
