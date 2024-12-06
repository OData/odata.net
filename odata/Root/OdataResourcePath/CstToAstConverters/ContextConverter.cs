namespace Root.OdataResourcePath.CstToAstConverters
{
    using Root;
    
    public sealed class ContextConverter :
        ConcreteSyntaxTreeNodes.Context.Visitor<
            AbstractSyntaxTreeNodes.Context,
            Void>
    {
        private ContextConverter()
        {
        }

        public static ContextConverter Default { get; } = new ContextConverter();
    }
}
