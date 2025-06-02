namespace NewStuff._Design._3_Context.Sample
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;

    public class EmployeeUpdater
    {
        private readonly ICollectionClr<User, string> usersClr;

        public EmployeeUpdater(ICollectionClr<User, string> usersClr)
        {
            this.usersClr = usersClr;
        }

        public async Task<Employee?> Update(string id, string displayName)
        {
            var user = new User(
                NullableProperty.Provided(displayName), 
                NonNullableProperty.NotProvided<IEnumerable<User>>(),
                NonNullableProperty.NotProvided<string>());
            var userClr = this
                .usersClr
                .Patch(id, user)
                .Expand(user => user.DirectReports) //// TODO select id
                .Select(user => user.DisplayName); //// TODO somehow note that chaining the select after the expand is something you can't do in linq

            var response = await userClr.Evaluate().ConfigureAwait(false);
            if (response == null)
            {
                return null;
            }

            if (response.TryAdapt(out var employee))
            {
                return employee;
            }

            //// TODO this feels pretty wrong here...
            return null;
        }
    }
}
