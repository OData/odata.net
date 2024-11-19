namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;

    public sealed class EntityCastOptionsTranslator :
        AbstractSyntaxTree.EntityCastOptions.Visitor<
            ConcreteSyntaxTree.EntityCastOptions,
            Void>
    {
        private EntityCastOptionsTranslator()
        {
        }

        public static EntityCastOptionsTranslator Default { get; } = new EntityCastOptionsTranslator();
    }
}
