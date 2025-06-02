namespace NewStuff._Design._3_Context.Sample
{
    using System.Threading.Tasks;

    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;

    public sealed class EmployeeGetter
    {
        private readonly ICollectionClr<User, string> usersClr;

        public EmployeeGetter(ICollectionClr<User, string> usersClr)
        {
            this.usersClr = usersClr;
        }

        public async Task<Employee> Get(string id)
        {
            var user = await this
                .usersClr
                .Get(id)
                //// TODO it'd be nice to be able to share the below "selectors" between the multi-valued getter and the single-valued getter
                .Expand(user => user.DirectReports) //// TODO select id
                .Select(user => user.DisplayName)
                .Evaluate()
                .ConfigureAwait(false);

            if (user.Value.TryAdapt(out var employee))
            {
                return employee;
            }

            throw new System.Exception("TODO no user with that id");
        }
    }
}
