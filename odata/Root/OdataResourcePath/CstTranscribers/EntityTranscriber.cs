namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;

    public sealed class EntityTranscriber
    {
        private EntityTranscriber()
        {
        }

        public static EntityTranscriber Default { get; } = new EntityTranscriber();

        public void Transcribe(Entity node, StringBuilder context)
        {
            context.Append("entity");
        }
    }
}
