namespace NewStuff._Design._2_Clr
{
    public class SingleValuedResponse<TEntity>
    {
        public SingleValuedResponse(TEntity value, string? type, string? id)
        {
            Value = value;
            Type = type;
            Id = id;
        }

        public TEntity Value { get; }

        public string? Type { get; } //// TODO this should be more strongly-typed

        public string? Id { get; } //// TODO this should be more strongly-typed
    }
}
