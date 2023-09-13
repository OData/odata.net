using DataSourceGenerator;
using System.ComponentModel.Design;
using Microsoft.Restier.Core.Submit;
using Microsoft.Restier.Core.Query;
using System.Linq.Expressions;

namespace ODataServiceTests
{
    [TestClass]
    public class DynamicDataSourceTests
    {
        public class Person
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public int age { get; set; }
        }

        public class Job
        {
            public string title { get; set; }
            public Person employee { get; set; }
        }

        public class Company
        {
            public string name { get; set; }
            public List<Person> employees { get; set; }
        }

        public class NWindTestData : DynamicDataSource
        {
            public static NWindTestData GetNWindTestData()
            {
                var sp = new ServiceContainer();
                sp.AddService(typeof(ISubmitExecutor), new MockSubmitExecutor());
                sp.AddService(typeof(IQueryExpressionSourcer), new MockQueryExpressionSourcer());
                sp.AddService(typeof(IChangeSetInitializer), new MockChangeSetInitializer());

                return new NWindTestData(sp);
            }

            public NWindTestData(IServiceProvider sp) : base(sp)
            {
                people = new List<Person>();
                company = new Company();
            }

            public List<Person> people { get; set; }
            public Company company { get; set; }    
        }

        public IServiceProvider buildServiceProvider()
        {
            var sp = new ServiceContainer();
            return sp;
        }

        [TestMethod]
        public void TestLoad()
        {
            var dataSource = NWindTestData.GetNWindTestData();

            var json = @"
                {
                    ""people"": [
                       {
                         ""firstName"":""Joe"",
                         ""lastName"":""Smith"",
                         ""age"":23
                       },
                       {
                         ""firstName"":""Jane"",
                         ""lastName"":""Doe"",
                         ""age"":21
                       }
                    ],
                    ""company"": 
                    {
                        ""name"":""widgets"",
                        ""employees"": [
                            {
                                ""firstName"":""Joe"",
                                ""lastName"":""Smith"",
                                ""age"":23
                            },
                            {
                                ""firstName"":""Jane"",
                                ""lastName"":""Doe"",
                                ""age"":21
                            }
                        ]
                    }
                }";

            dataSource.Load(json);
            Assert.AreEqual(2, dataSource.people.Count);
            Assert.AreEqual(2, dataSource.company.employees.Count);
        }
    }

    public class MockQueryExpressionSourcer : IQueryExpressionSourcer
    {
        public Expression ReplaceQueryableSource(QueryExpressionContext context, bool embedded)
        {
            throw new NotImplementedException();
        }
    }

    public class MockChangeSetInitializer : IChangeSetInitializer
    {
        public Task InitializeAsync(SubmitContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class MockSubmitExecutor : ISubmitExecutor
    {
        public Task<SubmitResult> ExecuteSubmitAsync(SubmitContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}