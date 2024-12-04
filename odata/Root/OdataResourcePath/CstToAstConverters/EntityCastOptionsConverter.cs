namespace Root.OdataResourcePath.CstToAstConverters
{
    using Root;

    public sealed class EntityCastOptionsConverter :
        ConcreteSyntaxTreeNodes.EntityCastOptions.Visitor<
            AbstractSyntaxTreeNodes.EntityCastOptions,
            Void>
    {
        private EntityCastOptionsConverter()
        {
        }

        public static EntityCastOptionsConverter Default { get; } = new EntityCastOptionsConverter();
    }
}
