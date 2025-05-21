namespace NewStuff._Design._4.Sample
{
    using System.Collections.Generic;
    using System.Linq;

    using NewStuff._Design._3_Context.Sample;

    public sealed class OrgChart : ITree<Employee>
    {
        private readonly EmployeeGetter employeeGetter;

        public OrgChart(Employee root, EmployeeGetter employeeGetter)
        {
            this.Data = root;
            this.employeeGetter = employeeGetter;
        }

        public Employee Data { get; }

        public IEnumerable<ITree<Employee>> Children
        {
            get
            {
                return this
                    .Data
                    .DirectReportIds
                    .Select(
                        directReportId => 
                            new OrgChart(
                                this.employeeGetter.Get(directReportId), 
                                this.employeeGetter));
            }
        }
    }
}
