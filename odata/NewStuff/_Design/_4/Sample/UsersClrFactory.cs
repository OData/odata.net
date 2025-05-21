namespace NewStuff._Design._4.Sample
{
    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;

    public static class UsersClrFactory
    {
        public static ICollectionClr<User, string> Create()
        {
            return new UsersClr();
        }
    }
}
