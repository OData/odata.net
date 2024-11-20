namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class QualifiedEntityTypeNameTranslator :
        AbstractSyntaxTreeNodes.QualifiedEntityTypeName.Visitor<
            ConcreteSyntaxTreeNodes.QualifiedEntityTypeName,
            Void>
    {
        private QualifiedEntityTypeNameTranslator()
        {
        }

        public static QualifiedEntityTypeNameTranslator Default { get; } = new QualifiedEntityTypeNameTranslator();
    }
}
