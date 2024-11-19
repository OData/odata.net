namespace Root.OdataResourcePath.CstToAstTranslators
{
    using System;

    public sealed class ResourcePathTranslator :
        ConcreteSyntaxTree.ResourcePath.Visitor<
            AbstractSyntaxTree.ResourcePath,
            Void>
    {
        private ResourcePathTranslator()
        {
        }

        public static ResourcePathTranslator Default { get; } = new ResourcePathTranslator();
    }
}
