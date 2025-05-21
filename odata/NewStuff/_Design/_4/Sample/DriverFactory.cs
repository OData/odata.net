namespace NewStuff._Design._4.Sample
{
    using System;
    using System.IO;

    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;
    using NewStuff._Design._3_Context.Sample;

    public static class DriverFactory
    {
        public static Driver Create()
        {
            var usersClr = UsersClrFactory.Create();
            var employeeContext = EmployeeContextFactory.Create(usersClr);
            var employeeGetter = EmployeeGetterFactory.Create(usersClr);
            var employeeAdder = EmployeeAdderFactory.Create(usersClr);
            var employeeUpdater = EmployeeUpdaterFactory.Create(usersClr);
            var textWriter = TextWriterFactory.Create();
            return new Driver(
                employeeContext,
                employeeGetter,
                employeeAdder,
                employeeUpdater,
                textWriter);
        }
    }

    public static class TextWriterFactory
    {
        public static TextWriter Create()
        {
            return Console.Out;
        }
    }

    public static class EmployeeUpdaterFactory
    {
        public static EmployeeUpdater Create(ICollectionClr<User, string> usersClr)
        {
            return new EmployeeUpdater(usersClr);
        }
    }

    public static class UsersClrFactory
    {
        public static ICollectionClr<User, string> Create()
        {

        }
    }

    public static class EmployeeGetterFactory
    {
        public static EmployeeGetter Create(ICollectionClr<User, string> usersClr)
        {
            return new EmployeeGetter(usersClr);
        }
    }

    public static class EmployeeAdderFactory
    {
        public static EmployeeAdder Create(ICollectionClr<User, string> usersClr)
        {
            return new EmployeeAdder(usersClr);
        }
    }

    public static class EmployeeContextFactory
    {
        public static EmployeeContext Create(ICollectionClr<User, string> usersClr)
        {
            var usersGetClr = usersClr.Get();
            return new EmployeeContext(usersGetClr);
        }
    }
}
