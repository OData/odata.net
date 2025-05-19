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
            return TrySelect<User, Employee>(usersReponse.Values.Select(user => user.Value), Adapt);
        }

        private static bool Adapt(User user, out Employee employee)
        {
            if (!(user.DisplayName is NullableProperty<string>.Provided displayName))
            {
                employee = default;
                return false;
            }

            if (!(user.DirectReports is NonNullableProperty<IEnumerable<User>>.Provided directReports))
            {
                employee = default;
                return false;
            }

            employee = new Employee(displayName.Value, TrySelect<User, string>(directReports.Value, Adapt));
            return true;
        }

        private static bool Adapt(User directReport, out string id)
        {
            if (!(directReport.Id is NonNullableProperty<string>.Provided provided))
            {
                id = default;
                return false;
            }

            id = provided.Value;
            return true;
        }

        public EmployeeContext Where(Expression<Func<Employee, bool>> predicate)
        {
            var adaptedPredicate = this.Adapt(predicate);
            var filteredClr = this.usersClr.Filter(adaptedPredicate);
            return new EmployeeContext(filteredClr);
        }

        private Expression<Func<User, bool>> Adapt(Expression<Func<Employee, bool>> toAdapt)
        {
            throw new Exception("TODO");
        }

        private static IEnumerable<TResult> TrySelect<TSource, TResult>(IEnumerable<TSource> source, Try<TSource, TResult> @try)
        {
            foreach (var element in source)
            {
                if (@try(element, out var result))
                {
                    yield return result;
                }
            }
        }

        private delegate bool Try<TInput, TOutput>(TInput input, out TOutput output);
    }
}
