namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTree;

    public sealed class BatchOptionsTranscriber : BatchOptions.Visitor<Void, StringBuilder>
    {
        private BatchOptionsTranscriber()
        {
        }

        public static BatchOptionsTranscriber Default { get; } = new BatchOptionsTranscriber();
    }
}
