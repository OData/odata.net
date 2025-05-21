namespace NewStuff._Design._2_Clr
{
    public class Entity<TEntity, TKey>
    {
        public TEntity Value { get; }

        public TKey Key { get; }
    }
}
