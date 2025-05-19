using System.Linq.Expressions;
using System;

namespace NewStuff._Design._2_Clr
{
    public interface IGetClr<TEntity>
    {
        ResponseEntity<TEntity> Evaluate();

        IGetCollectionClr<TEntity> Select<TProperty>(Expression<Func<TEntity, ResponseProperty<TProperty>>> selector);

        IGetCollectionClr<TEntity> Expand<TProperty>(Expression<Func<TEntity, ResponseProperty<TProperty>>> expander);
    }
}
