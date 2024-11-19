namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;
    
    public sealed class ContextTranslator :
        AbstractSyntaxTree.Context.Visitor<
            ConcreteSyntaxTree.Context,
            Void>
    {
        private ContextTranslator()
        {
        }

        public static ContextTranslator Default { get; } = new ContextTranslator();
    }
}
