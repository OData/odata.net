namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

    public sealed class EntityOptionsTranslator :
        ConcreteSyntaxTree.EntityOptions.Visitor<
            AbstractSyntaxTree.EntityOptions,
            Void>
    {
        private EntityOptionsTranslator()
        {
        }

        public static EntityOptionsTranslator Default { get; } = new EntityOptionsTranslator();
    }
}
