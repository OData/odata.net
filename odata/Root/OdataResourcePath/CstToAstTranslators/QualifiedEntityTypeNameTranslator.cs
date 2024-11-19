namespace Root.OdataResourcePath.CstToAstTranslators
{
    using System;

    public sealed class QualifiedEntityTypeNameTranslator :
        ConcreteSyntaxTree.QualifiedEntityTypeName.Visitor<
            AbstractSyntaxTree.QualifiedEntityTypeName,
            Void>
    {
        private QualifiedEntityTypeNameTranslator()
        {
        }

        public static QualifiedEntityTypeNameTranslator Default { get; } = new QualifiedEntityTypeNameTranslator();
    }
}
