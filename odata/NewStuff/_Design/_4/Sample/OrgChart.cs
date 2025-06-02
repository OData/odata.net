namespace NewStuff._Design._4.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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

        public IAsyncEnumerable<ITree<Employee>> Children
        {
            get
            {
                return SelectAsync(this
                    .Data
                    .DirectReportIds,
                    async directReportId =>
                        new OrgChart(
                            await this.employeeGetter.Get(directReportId).ConfigureAwait(false),
                            this.employeeGetter));
            }
        }

        private static async IAsyncEnumerable<TResult> SelectAsync<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            foreach (var element in source)
            {
                yield return await selector(element).ConfigureAwait(false);
            }
        }
    }
}
