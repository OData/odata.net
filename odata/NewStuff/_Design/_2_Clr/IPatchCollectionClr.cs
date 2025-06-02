namespace NewStuff._Design._2_Clr
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IPatchCollectionClr<TEntity>
    {
        Task<TEntity?> Evaluate();

        IPatchCollectionClr<TEntity> Select<TProperty>(Expression<Func<TEntity, Property<TProperty>>> selector);

        IPatchCollectionClr<TEntity> Expand<TProperty>(Expression<Func<TEntity, Property<TProperty>>> expander); //// TODO what about a nested filter?
    }
}
