namespace NewStuff._Design._4.Sample
{
    using System.IO;
    using System.Threading.Tasks;

    using NewStuff._Design._3_Context.Sample;

    public sealed class Driver
    {
        private readonly EmployeeContext employeeContext;
        private readonly EmployeeGetter employeeGetter;
        private readonly EmployeeAdder employeeAdder;
        private readonly EmployeeUpdater employeeUpdater;
        private readonly TextWriter textWriter;

        public Driver(
            EmployeeContext employeeContext, 
            EmployeeGetter employeeGetter,
            EmployeeAdder employeeAdder,
            EmployeeUpdater employeeUpdater,
            TextWriter textWriter)
        {
            this.employeeContext = employeeContext;
            this.employeeGetter = employeeGetter;
            this.employeeAdder = employeeAdder;
            this.employeeUpdater = employeeUpdater;
            this.textWriter = textWriter;
        }

        public async Task DoWork()
        {
            var employeesToUpdate = await employeeContext.Where(employee => employee.Name.StartsWith('g')).Evaluate().ConfigureAwait(false);
            foreach (var employeeToUpdate in employeesToUpdate)
            {
                await employeeUpdater.Update(employeeToUpdate.Id, employeeToUpdate.Name.Substring(1)).ConfigureAwait(false);
            }

            await employeeAdder.Add("some new employee").ConfigureAwait(false);

            var orgChart = new OrgChart(await employeeGetter.Get("CEO id").ConfigureAwait(false), this.employeeGetter);
            orgChart.Write(
                (writer, employee) =>
                {
                    writer.WriteLine(employee.Id);
                    writer.WriteLine(employee.Name);
                },
                this.textWriter);
        }
    }
}
