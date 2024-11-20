namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTree;

    public sealed class EntityCastOptionsTranscriber : EntityCastOptions.Visitor<Void, StringBuilder>
    {
        private EntityCastOptionsTranscriber()
        {
        }

        public static EntityCastOptionsTranscriber Default { get; } = new EntityCastOptionsTranscriber();
    }
}
