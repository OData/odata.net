namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class EntityCastOptionsConverter :
        AbstractSyntaxTreeNodes.EntityCastOptions.Visitor<
            ConcreteSyntaxTreeNodes.EntityCastOptions,
            Void>
    {
        private EntityCastOptionsConverter()
        {
        }

        public static EntityCastOptionsConverter Default { get; } = new EntityCastOptionsConverter();
    }
}
