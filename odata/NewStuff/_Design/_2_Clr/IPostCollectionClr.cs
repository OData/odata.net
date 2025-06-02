namespace NewStuff._Design._2_Clr
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IPostCollectionClr<TEntity>
    {
        Task<TEntity?> Evaluate(); //// TODO you need a better way to represent whether the created entity was returned

        IPostCollectionClr<TEntity> Select<TProperty>(Expression<Func<TEntity, Property<TProperty>>> selector);

        IPostCollectionClr<TEntity> Expand<TProperty>(Expression<Func<TEntity, Property<TProperty>>> expander); //// TODO what about a nested filter?
    }
}
