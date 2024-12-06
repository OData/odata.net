namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;
    
    public sealed class ContextConverter :
        AbstractSyntaxTreeNodes.Context.Visitor<
            ConcreteSyntaxTreeNodes.Context,
            Void>
    {
        private ContextConverter()
        {
        }

        public static ContextConverter Default { get; } = new ContextConverter();
    }
}
