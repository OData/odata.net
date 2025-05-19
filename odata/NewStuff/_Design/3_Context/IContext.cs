namespace NewStuff._Design._3_Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IContext<T>
    {
        IEnumerable<T> Evaluate();

        IContext<T> Where(Expression<Func<T, bool>> predicate);
    }
}
