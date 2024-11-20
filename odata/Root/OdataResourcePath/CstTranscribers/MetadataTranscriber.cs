namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTree;

    public sealed class MetadataTranscriber
    {
        private MetadataTranscriber()
        {
        }

        public static MetadataTranscriber Default { get; } = new MetadataTranscriber();

        public void Transcribe(Metadata node, StringBuilder context)
        {
            context.Append("$metadata");
        }
    }
}
