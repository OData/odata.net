namespace NewStuff._Design._2_Clr
{
    public class SingleValuedResponse<TEntity>
    {
        public TEntity Value { get; }

        public string? Type { get; } //// TODO this should be more strongly-typed

        public string? Id { get; } //// TODO this should be more strongly-typed
    }
}
