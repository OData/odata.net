namespace NewStuff._Design._2_Clr
{
    public interface ICollectionClr<T>
    {
        ICollectionClr<T> Get();

        IPostCollectionClr<T> Post(T entity); //// TODO what about deep insert and deep update?
    }
}
