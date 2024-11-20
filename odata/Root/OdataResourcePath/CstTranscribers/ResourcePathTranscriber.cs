namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;

    public sealed class ResourcePathTranscriber : ResourcePath.Visitor<Void, StringBuilder>
    {
        private ResourcePathTranscriber()
        {
        }

        public static ResourcePathTranscriber Default { get; } = new ResourcePathTranscriber();
    }
}
