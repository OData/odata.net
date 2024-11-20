namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTree;

    public sealed class QualifiedEntityTypeNameTranscriber : QualifiedEntityTypeName.Visitor<Void, StringBuilder>
    {
        private QualifiedEntityTypeNameTranscriber()
        {
        }

        public static QualifiedEntityTypeNameTranscriber Default { get; } = new QualifiedEntityTypeNameTranscriber();
    }
}
