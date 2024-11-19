namespace Root.OdataResourcePath.CstToAstTranslators
{
    using System;

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
