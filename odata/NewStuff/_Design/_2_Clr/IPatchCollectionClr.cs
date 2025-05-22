namespace NewStuff._Design._2_Clr
{
    using System;
    using System.Linq.Expressions;

    public interface IPatchCollectionClr<TEntity>
    {
        TEntity? Evaluate();

        IPatchCollectionClr<TEntity> Select<TProperty>(Expression<Func<TEntity, Property<TProperty>>> selector);

        IPatchCollectionClr<TEntity> Expand<TProperty>(Expression<Func<TEntity, Property<TProperty>>> expander); //// TODO what about a nested filter?
    }
}
