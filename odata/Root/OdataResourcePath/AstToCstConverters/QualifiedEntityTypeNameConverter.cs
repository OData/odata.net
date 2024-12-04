namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class QualifiedEntityTypeNameConverter :
        AbstractSyntaxTreeNodes.QualifiedEntityTypeName.Visitor<
            ConcreteSyntaxTreeNodes.QualifiedEntityTypeName,
            Void>
    {
        private QualifiedEntityTypeNameConverter()
        {
        }

        public static QualifiedEntityTypeNameConverter Default { get; } = new QualifiedEntityTypeNameConverter();
    }
}
