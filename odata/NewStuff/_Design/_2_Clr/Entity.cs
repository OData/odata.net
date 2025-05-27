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

    //// TODO should there be a "keycomponentproperty" type, and then `entity` has a `key` property that combines the components
}
