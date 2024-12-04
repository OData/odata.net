namespace Root.OdataResourcePath.Transcribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;

    public sealed class EntityCastOptionsTranscriber : EntityCastOptions.Visitor<Void, StringBuilder>
    {
        private EntityCastOptionsTranscriber()
        {
        }

        public static EntityCastOptionsTranscriber Default { get; } = new EntityCastOptionsTranscriber();
    }
}
