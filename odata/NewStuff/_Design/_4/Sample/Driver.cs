namespace NewStuff._Design._4.Sample
{
    using System.IO;

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

        public void DoWork()
        {
            var employeesToUpdate = employeeContext.Where(employee => employee.Name.StartsWith('g')).Evaluate();
            foreach (var employeeToUpdate in employeesToUpdate)
            {
                employeeUpdater.Update(employeeToUpdate.Id, employeeToUpdate.Name.Substring(1));
            }

            employeeAdder.Add("some new employee");

            var orgChart = new OrgChart(employeeGetter.Get("CEO id"), this.employeeGetter);
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
