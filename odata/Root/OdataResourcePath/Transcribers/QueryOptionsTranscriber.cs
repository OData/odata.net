namespace Root.OdataResourcePath.Transcribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;

    public sealed class QueryOptionsTranscriber : QueryOptions.Visitor<Void, StringBuilder>
    {
        private QueryOptionsTranscriber()
        {
        }

        public static QueryOptionsTranscriber Default { get; } = new QueryOptionsTranscriber();
    }
}
