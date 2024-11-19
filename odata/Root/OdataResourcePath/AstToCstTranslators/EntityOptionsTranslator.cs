namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;

    public sealed class EntityOptionsTranslator :
        AbstractSyntaxTree.EntityOptions.Visitor<
            ConcreteSyntaxTree.EntityOptions,
            Void>
    {
        private EntityOptionsTranslator()
        {
        }

        public static EntityOptionsTranslator Default { get; } = new EntityOptionsTranslator();
    }
}
