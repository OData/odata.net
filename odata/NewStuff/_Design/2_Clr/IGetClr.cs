using System.Linq.Expressions;
using System;

namespace NewStuff._Design._2_Clr
{
    public interface IGetClr<TEntity, TKey>
    {
        SingleValuedResponse<TEntity> Evaluate();

        IGetClr<TEntity, TKey> Select<TProperty>(Expression<Func<TEntity, Property<TProperty>>> selector);

        IGetClr<TEntity, TKey> Expand<TProperty>(Expression<Func<TEntity, Property<TProperty>>> expander);
    }
}
