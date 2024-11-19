namespace Root.OdataResourcePath.CstToAstTranslators
{
    using System;

    public sealed class MetadataOptionsTranslator :
        ConcreteSyntaxTree.MetadataOptions.Visitor<
            AbstractSyntaxTree.MetadataOptions,
            Void>
    {
        private MetadataOptionsTranslator()
        {
        }

        public static MetadataOptionsTranslator Default { get; } = new MetadataOptionsTranslator();
    }
}
