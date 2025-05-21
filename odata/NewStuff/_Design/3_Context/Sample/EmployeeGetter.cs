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
                .Expand(user => user.DirectReports) //// TODO select id
                .Select(user => user.DisplayName)
                .Evaluate();

            if (EmployeeContext.Adapt(user.Value, out var employee))
            {
                return employee;
            }

            throw new System.Exception("TODO no user with that id");
        }
    }
}
