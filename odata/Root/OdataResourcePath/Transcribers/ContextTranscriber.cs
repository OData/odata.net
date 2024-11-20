namespace Root.OdataResourcePath.Transcribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;

    public sealed class ContextTranscriber : Context.Visitor<Void, StringBuilder>
    {
        private ContextTranscriber()
        {
        }

        public static ContextTranscriber Default { get; } = new ContextTranscriber();
    }
}
