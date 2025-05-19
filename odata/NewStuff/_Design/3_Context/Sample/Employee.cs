namespace NewStuff._Design._3_Context.Sample
{
    using NewStuff._Design._2_Clr.Sample;
    using System.Collections.Generic;

    public class Employee
    {
        public Employee(string name, IEnumerable<string> directReportIds)
        {
            this.Name = name;
            this.DirectReportIds = directReportIds;
        }

        public string Name { get; }
        public IEnumerable<string> DirectReportIds { get; }
    }
}
