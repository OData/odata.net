namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class EntityCastOptionsTranslator :
        AbstractSyntaxTreeNodes.EntityCastOptions.Visitor<
            ConcreteSyntaxTreeNodes.EntityCastOptions,
            Void>
    {
        private EntityCastOptionsTranslator()
        {
        }

        public static EntityCastOptionsTranslator Default { get; } = new EntityCastOptionsTranslator();
    }
}
