namespace Root.OdataResourcePath.CstToAstConverters
{
    using Root;

    public sealed class QualifiedEntityTypeNameConverter :
        ConcreteSyntaxTreeNodes.QualifiedEntityTypeName.Visitor<
            AbstractSyntaxTreeNodes.QualifiedEntityTypeName,
            Void>
    {
        private QualifiedEntityTypeNameConverter()
        {
        }

        public static QualifiedEntityTypeNameConverter Default { get; } = new QualifiedEntityTypeNameConverter();
    }
}
