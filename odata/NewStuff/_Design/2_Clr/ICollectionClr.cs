namespace NewStuff._Design._2_Clr
{
    public interface ICollectionClr<TEntity, TKey>
    {
        IGetCollectionClr<TEntity, TKey> Get();

        IGetClr<TEntity> Get(TKey key);

        IPostCollectionClr<TEntity> Post(TEntity entity); //// TODO what about deep insert and deep update? //// TODO what about services that allow the key to be provided on create?
    }
}
