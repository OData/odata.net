namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class EntityOptionsConverter :
        AbstractSyntaxTreeNodes.EntityOptions.Visitor<
            ConcreteSyntaxTreeNodes.EntityOptions,
            Void>
    {
        private EntityOptionsConverter()
        {
        }

        public static EntityOptionsConverter Default { get; } = new EntityOptionsConverter();
    }
}
