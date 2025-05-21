namespace NewStuff._Design._4.Sample
{
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
}
