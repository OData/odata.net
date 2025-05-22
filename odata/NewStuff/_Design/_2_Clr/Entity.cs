namespace NewStuff._Design._2_Clr
{
    public class Entity<TEntity>
    {
        public Entity(TEntity value)
        {
            Value = value;
        }

        public TEntity Value { get; }
    }
}
