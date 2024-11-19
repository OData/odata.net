namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

    public sealed class EntityCastOptionsTranslator :
        ConcreteSyntaxTree.EntityCastOptions.Visitor<
            AbstractSyntaxTree.EntityCastOptions,
            Void>
    {
        private EntityCastOptionsTranslator()
        {
        }

        public static EntityCastOptionsTranslator Default { get; } = new EntityCastOptionsTranslator();
    }
}
