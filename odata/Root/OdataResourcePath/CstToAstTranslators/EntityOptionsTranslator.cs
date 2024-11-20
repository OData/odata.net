namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

    public sealed class EntityOptionsTranslator :
        ConcreteSyntaxTreeNodes.EntityOptions.Visitor<
            AbstractSyntaxTreeNodes.EntityOptions,
            Void>
    {
        private EntityOptionsTranslator()
        {
        }

        public static EntityOptionsTranslator Default { get; } = new EntityOptionsTranslator();
    }
}
