namespace NewStuff._Design._3_Context.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class EmployeeContext : IContext<Employee>
    {
        public EmployeeContext()
        {
        }

        public IEnumerable<Employee> Evaluate()
        {
            throw new NotImplementedException();
        }

        public IContext<Employee> Where(Expression<Func<Employee, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
