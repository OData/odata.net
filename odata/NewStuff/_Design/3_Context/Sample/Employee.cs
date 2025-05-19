namespace NewStuff._Design._3_Context.Sample
{
    using System.Collections.Generic;

    public class Employee
    {
        public Employee(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
        public IEnumerable<string> DirectReportIds { get; }
    }
}
