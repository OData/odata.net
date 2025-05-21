namespace NewStuff._Design._2_Clr
{
    using System;
    using System.Linq.Expressions;

    public interface IGetCollectionClr<TEntity, TKey>
    {
        CollectionResponse<TEntity, TKey> Evaluate();

        IGetCollectionClr<TEntity, TKey> Filter(Expression<Func<TEntity, bool>> predicate); //// TODO how to prevent the use of control information in the predicate?

        IGetCollectionClr<TEntity, TKey> Select<TProperty>(Expression<Func<TEntity, Property<TProperty>>> selector);

        IGetCollectionClr<TEntity, TKey> Expand<TProperty>(Expression<Func<TEntity, Property<TProperty>>> expander); //// TODO what about a nested filter? //// TODO what about a nested select

        IGetCollectionClr<TEntity, TKey> Skip(int count);
    }
}
