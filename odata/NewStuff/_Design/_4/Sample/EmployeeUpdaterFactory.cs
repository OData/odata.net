namespace NewStuff._Design._4.Sample
{
    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;
    using NewStuff._Design._3_Context.Sample;

    public static class EmployeeUpdaterFactory
    {
        public static EmployeeUpdater Create(ICollectionClr<User, string> usersClr)
        {
            return new EmployeeUpdater(usersClr);
        }
    }
}
