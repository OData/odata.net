namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class EntityOptionsTranslator :
        AbstractSyntaxTreeNodes.EntityOptions.Visitor<
            ConcreteSyntaxTreeNodes.EntityOptions,
            Void>
    {
        private EntityOptionsTranslator()
        {
        }

        public static EntityOptionsTranslator Default { get; } = new EntityOptionsTranslator();
    }
}
