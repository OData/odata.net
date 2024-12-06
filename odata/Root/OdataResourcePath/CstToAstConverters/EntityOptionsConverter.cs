namespace Root.OdataResourcePath.CstToAstConverters
{
    using Root;

    public sealed class EntityOptionsConverter :
        ConcreteSyntaxTreeNodes.EntityOptions.Visitor<
            AbstractSyntaxTreeNodes.EntityOptions,
            Void>
    {
        private EntityOptionsConverter()
        {
        }

        public static EntityOptionsConverter Default { get; } = new EntityOptionsConverter();
    }
}
