namespace NewStuff._Design._2_Clr
{
    using System;
    using System.Linq.Expressions;

    public interface IGetCollectionClr<TEntity>
    {
        CollectionResponse<TEntity> Evaluate();

        IGetCollectionClr<TEntity> Filter(Expression<Func<TEntity, bool>> predicate); //// TODO how to prevent the use of control information in the predicate?

        IGetCollectionClr<TEntity> Select<TProperty>(Expression<Func<TEntity, Property<TProperty>>> selector);

        IGetCollectionClr<TEntity> Expand<TProperty>(Expression<Func<TEntity, Property<TProperty>>> expander); //// TODO what about a nested filter?

        IGetCollectionClr<TEntity> Skip(int count);
    }
}
