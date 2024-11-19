namespace Root.OdataResourcePath.CstToAstTranslators
{
    using System;

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
