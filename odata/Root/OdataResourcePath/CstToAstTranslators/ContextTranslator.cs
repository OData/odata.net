namespace Root.OdataResourcePath.CstToAstTranslators
{
    using System;

    public sealed class ContextTranslator :
        ConcreteSyntaxTree.Context.Visitor<
            AbstractSyntaxTree.Context,
            Void>
    {
        private ContextTranslator()
        {
        }

        public static ContextTranslator Default { get; } = new ContextTranslator();
    }
}
