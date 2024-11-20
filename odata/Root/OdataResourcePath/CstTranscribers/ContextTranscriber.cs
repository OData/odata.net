namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTree;

    public sealed class ContextTranscriber : Context.Visitor<Void, StringBuilder>
    {
        private ContextTranscriber()
        {
        }

        public static ContextTranscriber Default { get; } = new ContextTranscriber();
    }
}
