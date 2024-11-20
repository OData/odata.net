namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTree;

    public sealed class EntityOptionsTranscriber : EntityOptions.Visitor<Void, StringBuilder>
    {
        private EntityOptionsTranscriber()
        {
        }

        public static EntityOptionsTranscriber Default { get; } = new EntityOptionsTranscriber();
    }
}
