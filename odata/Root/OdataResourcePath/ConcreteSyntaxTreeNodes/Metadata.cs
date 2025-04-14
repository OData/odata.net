namespace Root.OdataResourcePath.ConcreteSyntaxTreeNodes
{
    public sealed class Metadata
    {
        private Metadata()
        {
        }

        public static Metadata Instance { get; } = new Metadata();
    }
}
