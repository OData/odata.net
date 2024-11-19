namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;

    public sealed class QualifiedEntityTypeNameTranslator :
        AbstractSyntaxTree.QualifiedEntityTypeName.Visitor<
            ConcreteSyntaxTree.QualifiedEntityTypeName,
            Void>
    {
        private QualifiedEntityTypeNameTranslator()
        {
        }

        public static QualifiedEntityTypeNameTranslator Default { get; } = new QualifiedEntityTypeNameTranslator();
    }
}
