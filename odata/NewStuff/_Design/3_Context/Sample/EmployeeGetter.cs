namespace NewStuff._Design._3_Context.Sample
{
    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;

    public sealed class EmployeeGetter
    {
        private readonly IClr<User, string> userClr;

        public EmployeeGetter(IClr<User, string> userClr)
        {
            this.userClr = userClr;
        }

        public Employee Get(string id)
        {
            var user = this
                .userClr
                .Get(id)
                //// TODO it'd be nice to be able to share the below "selectors" between the multi-valued getter and the single-valued getter
                .Expand(user => user.DirectReports) //// TODO select id
                .Select(user => user.DisplayName)
                .Evaluate();

            if (user.Value.TryAdapt(out var employee))
            {
                return employee;
            }

            throw new System.Exception("TODO no user with that id");
        }
    }
}
