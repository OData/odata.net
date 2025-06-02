namespace NewStuff._Design._2_Clr
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IGetClr<TEntity, TKey>
    {
        Task<SingleValuedResponse<TEntity>> Evaluate();

        IGetClr<TEntity, TKey> Select<TProperty>(Expression<Func<TEntity, Property<TProperty>>> selector);

        IGetClr<TEntity, TKey> Expand<TProperty>(Expression<Func<TEntity, Property<TProperty>>> expander);
    }
}
