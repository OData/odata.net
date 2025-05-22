namespace NewStuff._Design._2_Clr
{
    public class CollectionResponseEntity<TEntity>
    {
        public CollectionResponseEntity(Entity<TEntity> value, string? type, string? id)
        {
            Value = value;
            Type = type;
            Id = id;
        }

        public Entity<TEntity> Value { get; }

        public string? Type { get; } //// TODO this should be more strongly-typed

        public string? Id { get; } //// TODO this should be more strongly-typed
    }
}
