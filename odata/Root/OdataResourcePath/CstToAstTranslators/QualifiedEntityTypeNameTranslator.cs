namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

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
