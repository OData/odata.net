namespace NewStuff._Design._4.Sample
{
    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;
    using NewStuff._Design._3_Context.Sample;

    public static class EmployeeGetterFactory
    {
        public static EmployeeGetter Create(ICollectionClr<User, string> usersClr)
        {
            return new EmployeeGetter(usersClr);
        }
    }
}
