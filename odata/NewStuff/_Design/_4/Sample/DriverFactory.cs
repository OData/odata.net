namespace NewStuff._Design._4.Sample
{
    using System;

    using NewStuff._Design._0_Convention.Sample;

    public static class DriverFactory
    {
        public static Driver Create(Func<IHttpClient> httpClientFactory)
        {
            var usersClr = UsersClrFactory.Create(httpClientFactory);
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
