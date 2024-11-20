namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;

    public sealed class BatchTranscriber
    {
        private BatchTranscriber()
        {
        }

        public static BatchTranscriber Default { get; } = new BatchTranscriber();

        public void Transcribe(Batch node, StringBuilder context)
        {
            context.Append("$batch");
        }
    }
}
