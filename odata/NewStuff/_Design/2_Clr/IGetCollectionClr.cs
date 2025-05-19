namespace NewStuff._Design._2_Clr
{
    using System;
    using System.Linq.Expressions;

    public interface IGetCollectionClr<TEntity>
    {
        CollectionResponse<TEntity> Evaluate();

        ICollectionClr<TEntity> Filter(Expression<Func<TEntity, bool>> predicate); //// TODO how to prevent the use of control information in the predicate?

        ICollectionClr<TEntity> Select<TProperty>(Expression<Func<TEntity, NullableProperty<TProperty>>> selector);

        ICollectionClr<TEntity> Expand<TProperty>(Expression<Func<TEntity, NullableProperty<TProperty>>> expander); //// TODO what about a nested filter?

        ICollectionClr<TEntity> Skip(int count);
    }
}
