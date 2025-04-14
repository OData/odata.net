namespace Root.OdataResourcePath.ConcreteSyntaxTreeNodes
{
    public sealed class Batch
    {
        private Batch()
        {
        }

        public static Batch Instance { get; } = new Batch();
    }
}
