namespace Root.OdataResourcePath.Transcribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;

    public sealed class QualifiedEntityTypeNameTranscriber : QualifiedEntityTypeName.Visitor<Void, StringBuilder>
    {
        private QualifiedEntityTypeNameTranscriber()
        {
        }

        public static QualifiedEntityTypeNameTranscriber Default { get; } = new QualifiedEntityTypeNameTranscriber();
    }
}
