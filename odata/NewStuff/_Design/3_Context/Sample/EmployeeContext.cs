namespace NewStuff._Design._3_Context.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;
    
    public class EmployeeContext : IContext<Employee>
    {
        private readonly IGetCollectionClr<User> usersClr;

        public EmployeeContext(IGetCollectionClr<User> usersClr)
        {
            this.usersClr = usersClr;
        }

        public IEnumerable<Employee> Evaluate()
        {
            var usersWithDirectReportsClr = this.usersClr.Expand(user => user.DirectReports);
            var usersReponse = usersWithDirectReportsClr.Evaluate();

            return usersReponse
                .Values
                .Select(
                    user => new Employee(
                        user.Value.DisplayName.Value, 
                        user.Value.DirectReports.Value.Select(directReport => directReport.Id.Value)));
        }

        public IContext<Employee> Where(Expression<Func<Employee, bool>> predicate)
        {
            var adaptedPredicate = this.Adapt(predicate);
            var filteredClr = this.usersClr.Filter(adaptedPredicate);
            return new EmployeeContext(filteredClr);
        }

        private Expression<Func<User, bool>> Adapt(Expression<Func<Employee, bool>> toAdapt)
        {
            throw new Exception("TODO");
        }
    }
}
