namespace NewStuff._Design._3_Context.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;
    
    public class EmployeeContext : IContext<Employee>
    {
        private readonly ICollectionClr<User> clr;

        public EmployeeContext(ICollectionClr<User> clr)
        {
            this.clr = clr;
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
