namespace NewStuff._Design._2_Clr
{
    public class CollectionResponseEntity<TEntity, TKey>
    {
        public Entity<TEntity, TKey> Value { get; }

        public string? Type { get; } //// TODO this should be more strongly-typed

        public string? Id { get; } //// TODO this should be more strongly-typed
    }
}
