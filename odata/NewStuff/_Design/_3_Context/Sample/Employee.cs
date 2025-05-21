namespace NewStuff._Design._3_Context.Sample
{
    using System.Collections.Generic;

    public class Employee
    {
        public Employee(string id, string name, IEnumerable<string> directReportIds)
        {
            this.Id = id;
            this.Name = name;
            this.DirectReportIds = directReportIds;
        }

        public string Id { get; }
        public string Name { get; }
        public IEnumerable<string> DirectReportIds { get; }
    }
}
