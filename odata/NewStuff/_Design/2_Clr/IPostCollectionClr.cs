namespace NewStuff._Design._2_Clr
{
    using System;
    using System.Linq.Expressions;

    public interface IPostCollectionClr<TEntity>
    {
        TEntity? Evaluate(); //// TODO you need a better way to represent whether the created entity was returned
        
        ICollectionClr<TEntity> Select<TProperty>(Expression<Func<TEntity, Property<TProperty>>> selector);

        ICollectionClr<TEntity> Expand<TProperty>(Expression<Func<TEntity, Property<TProperty>>> expander); //// TODO what about a nested filter?
    }
}
