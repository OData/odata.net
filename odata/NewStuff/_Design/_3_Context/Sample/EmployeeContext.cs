namespace NewStuff._Design._3_Context.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;
    
    public class EmployeeContext
    {
        private readonly IGetCollectionClr<User> usersClr;

        public EmployeeContext(IGetCollectionClr<User> usersClr)
        {
            this.usersClr = usersClr;
        }

        public IEnumerable<Employee> Evaluate()
        {
            var usersWithDirectReportsClr = this
                .usersClr
                .Expand(user => user.DirectReports) //// TODO select id
                .Select(user => user.DisplayName);
            var usersReponse = usersWithDirectReportsClr.Evaluate();

            //// TODO traverse skiptokens

            //// TODO note that you are skipping users that didn't have displayname or directreports provided and you are skipping direct reports that didn't have id provided even though, based on your request, these things should all be provided in the response; this is because you have no way to surface those errors
            return usersReponse
                .Values
                .Select(user => user.Value.Value)
                .TrySelect<User, Employee>(UserExtensions.TryAdapt);
        }

        public EmployeeContext Where(Expression<Func<Employee, bool>> predicate)
        {
            var adaptedPredicate = this.Adapt(predicate);
            var filteredClr = this.usersClr.Filter(adaptedPredicate);
            return new EmployeeContext(filteredClr);
        }

        private Expression<Func<User, bool>> Adapt(Expression<Func<Employee, bool>> toAdapt)
        {
            //// TODO
            return null;
        }
    }
}
