namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

    public sealed class QualifiedEntityTypeNameTranslator :
        ConcreteSyntaxTreeNodes.QualifiedEntityTypeName.Visitor<
            AbstractSyntaxTreeNodes.QualifiedEntityTypeName,
            Void>
    {
        private QualifiedEntityTypeNameTranslator()
        {
        }

        public static QualifiedEntityTypeNameTranslator Default { get; } = new QualifiedEntityTypeNameTranslator();
    }
}
