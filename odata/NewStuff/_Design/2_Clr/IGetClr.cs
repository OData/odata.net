using System.Linq.Expressions;
using System;

namespace NewStuff._Design._2_Clr
{
    public interface IGetClr<TEntity>
    {
        SingleValuedResponse<TEntity> Evaluate();

        ICollectionClr<TEntity> Select<TProperty>(Expression<Func<TEntity, NullableProperty<TProperty>>> selector);

        ICollectionClr<TEntity> Expand<TProperty>(Expression<Func<TEntity, NullableProperty<TProperty>>> expander);
    }
}
