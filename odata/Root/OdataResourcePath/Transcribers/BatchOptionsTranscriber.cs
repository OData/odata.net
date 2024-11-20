namespace Root.OdataResourcePath.Transcribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;

    public sealed class BatchOptionsTranscriber : BatchOptions.Visitor<Void, StringBuilder>
    {
        private BatchOptionsTranscriber()
        {
        }

        public static BatchOptionsTranscriber Default { get; } = new BatchOptionsTranscriber();
    }
}
