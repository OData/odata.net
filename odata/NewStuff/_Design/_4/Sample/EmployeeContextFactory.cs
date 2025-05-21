namespace NewStuff._Design._4.Sample
{
    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;
    using NewStuff._Design._3_Context.Sample;

    public static class EmployeeContextFactory
    {
        public static EmployeeContext Create(ICollectionClr<User, string> usersClr)
        {
            var usersGetClr = usersClr.Get();
            return new EmployeeContext(usersGetClr);
        }
    }
}
