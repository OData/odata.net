using System.Linq.Expressions;
using System;

namespace NewStuff._Design._2_Clr
{
    public interface IGetClr<TEntity>
    {
        SingleValuedResponse<TEntity> Evaluate();

        IClr<TEntity> Select<TProperty>(Expression<Func<TEntity, Property<TProperty>>> selector);

        IClr<TEntity> Expand<TProperty>(Expression<Func<TEntity, Property<TProperty>>> expander);
    }
}
