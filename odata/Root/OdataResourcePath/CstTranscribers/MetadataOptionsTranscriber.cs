namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;

    public sealed class MetadataOptionsTranscriber : MetadataOptions.Visitor<Void, StringBuilder>
    {
        private MetadataOptionsTranscriber()
        {
        }

        public static MetadataOptionsTranscriber Default { get; } = new MetadataOptionsTranscriber();
    }
}
