namespace Root.OdataResourcePath.ConcreteSyntaxTreeNodes
{
    public sealed class Entity
    {
        private Entity()
        {
        }

        public static Entity Instance { get; } = new Entity();
    }
}
