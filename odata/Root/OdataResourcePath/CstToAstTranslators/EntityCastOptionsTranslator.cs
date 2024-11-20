namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

    public sealed class EntityCastOptionsTranslator :
        ConcreteSyntaxTreeNodes.EntityCastOptions.Visitor<
            AbstractSyntaxTreeNodes.EntityCastOptions,
            Void>
    {
        private EntityCastOptionsTranslator()
        {
        }

        public static EntityCastOptionsTranslator Default { get; } = new EntityCastOptionsTranslator();
    }
}
