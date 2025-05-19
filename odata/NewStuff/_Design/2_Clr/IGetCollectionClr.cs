namespace NewStuff._Design._2_Clr
{
    using System;
    using System.Linq.Expressions;

    public interface IGetCollectionClr<TEntity>
    {
        CollectionResponse<TEntity> Evaluate();

        IGetCollectionClr<TEntity> Filter(Expression<Func<TEntity, bool>> predicate);

        IGetCollectionClr<TEntity> Select<TProperty>(Expression<Func<TEntity, ResponseProperty<TProperty>>> selector);

        IGetCollectionClr<TEntity> Expand<TProperty>(Expression<Func<TEntity, ResponseProperty<TProperty>>> expander);

        IGetCollectionClr<TEntity> Skip(int count);
    }
}
