namespace NewStuff._Design._2_Clr
{
    public interface IClr<TEntity, TKey>
    {
        IGetClr<TEntity, TKey> Get(TKey key);
    }
}
