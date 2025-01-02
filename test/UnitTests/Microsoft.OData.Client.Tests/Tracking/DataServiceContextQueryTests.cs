//---------------------------------------------------------------------
// <copyright file="DataServiceContextQueryTests.cs" company="Microsoft">
// Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Microsoft.OData.Client.Tests.Tracking
{
    public class DataServiceContextQueryTests
    {
        private const string ServiceRoot = "http://localhost:8007";
        private readonly Container _defaultContext;

        #region Test Edmx
        private const string Edmx = @"<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
    <edmx:DataServices>
        <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Sample.API.Models"">
            <EntityType Name=""Employee"">
                <Key>
                    <PropertyRef Name=""EmpNumber"" />
                    <PropertyRef Name=""EmpType"" />
                    <PropertyRef Name=""OrgId"" />
                </Key>
                <Property Name=""EmpNumber"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""EmpType"" Type=""Sample.API.Models.Enums.EmployeeType"" Nullable=""false"" />
                <Property Name=""OrgId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
                <Property Name=""Salary"" Type=""Edm.Decimal"" Nullable=""false"" Scale=""Variable"" />
                <NavigationProperty Name=""Organization"" Type=""Sample.API.Models.Organization"" Nullable=""false"">
                    <ReferentialConstraint Property=""OrgId"" ReferencedProperty=""Id"" />
                </NavigationProperty>
            </EntityType>
            <EntityType Name=""Organization"">
                <Key>
                    <PropertyRef Name=""Id"" />
                </Key>
                <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
            </EntityType>
        </Schema>
        <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm""
            Namespace=""Sample.API.Models.Enums"">
            <EnumType Name=""EmployeeType"">
                <Member Name=""None"" Value=""1"" />
                <Member Name=""FullTime"" Value=""2"" />
                <Member Name=""PartTime"" Value=""3"" />
            </EnumType>
        </Schema>
        <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Default"">
            <EntityContainer Name=""Container"">
                <EntitySet Name=""Employees"" EntityType=""Sample.API.Models.Employee"" />
            </EntityContainer>
        </Schema>
    </edmx:DataServices>
</edmx:Edmx>";
        #endregion

        public DataServiceContextQueryTests()
        {
            var uri = new Uri(ServiceRoot);
            _defaultContext = new Container(uri);
        }

        [Fact]
        public async Task SelectEntities_WithEnumAsKey_DoNotThrowException()
        {
            // Arrange
            var expectedUri = $"{ServiceRoot}/Employees";

            string response = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#Employees"",
    ""value"": [
        {
            ""EmpNumber"": 1,
            ""EmpType"": ""FullTime"",
            ""OrgId"": 1,
            ""Name"": ""John Doe""
        },
        {
            ""EmpNumber"": 2,
            ""EmpType"": ""PartTime"",
            ""OrgId"": 1,
            ""Name"": ""Jane Doe""
        }
    ]
}";
            SetupContextWithRequestPipeline(_defaultContext, response, "Employees");
            _defaultContext.SendingRequest2 += (sender, args) =>
            {
                Assert.Equal(expectedUri, args.RequestMessage.Url.ToString());
            };

            // Act
            DataServiceQuery<Employee> query = _defaultContext.Employees;
            IEnumerable<Employee> employees = await query.ExecuteAsync();

            // Assert
            Assert.Equal(expectedUri, query.ToString());
            Assert.Equal(2, employees.Count());
        }

        [Fact]
        public async Task DetachAndAttachEntity_WithEnumAsKey_DoNotThrowException()
        {
            // Arrange
            var expectedUri = $"{ServiceRoot}/Employees";

            string response = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#Employees"",
    ""value"": [
        {
            ""EmpNumber"": 1,
            ""EmpType"": ""FullTime"",
            ""OrgId"": 1,
            ""Name"": ""John Doe""
        },
        {
            ""EmpNumber"": 2,
            ""EmpType"": ""PartTime"",
            ""OrgId"": 1,
            ""Name"": ""Jane Doe""
        }
    ]
}";
            SetupContextWithRequestPipeline(_defaultContext, response, "Employees");
            _defaultContext.SendingRequest2 += (sender, args) =>
            {
                Assert.Equal(expectedUri, args.RequestMessage.Url.ToString());
            };

            // Act
            var employeeCollection = new DataServiceCollection<Employee>(_defaultContext.Employees);

            // Get the first entity from the context
            object entity = _defaultContext.Entities.First().Entity;

            // Remove the entity from the context
            _defaultContext.Detach(entity);

            // Attach the entity back to the context
            var exception = Record.Exception(() => _defaultContext.AttachTo("Employees", entity));

            // Assert
            Assert.Null(exception);
            Assert.Equal(2, employeeCollection.Count());

            DataServiceQuery<Employee> query = _defaultContext.Employees;
            IEnumerable<Employee> employees = await query.ExecuteAsync();

            Assert.Equal(expectedUri, query.ToString());
            Assert.Equal(2, employees.Count());
        }

        [Fact]
        public void UseWhereToFilterByOtherKeyOtherThanEnumKey_WithEnumAsKey_DoNotThrowException()
        {
            // Arrange
            string expectedUri = $"{ServiceRoot}/Employees?$filter=EmpNumber eq 8";

            string response = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#Employees"",
    ""value"": [
        {
            ""EmpNumber"": 8,
            ""EmpType"": ""PartTime"",
            ""OrgId"": 1,
            ""Name"": ""Employee Two""
        }
    ]
}";
            SetupContextWithRequestPipeline(_defaultContext, response, "Employees");
            _defaultContext.SendingRequest2 += (sender, args) =>
            {
                Assert.Equal($"{expectedUri}&$top=1", args.RequestMessage.Url.ToString());
            };

            // Act
            IQueryable<Employee> query = _defaultContext.Employees.Where(e => e.EmpNumber == 8);
            Employee employee = query.First();

            // Assert
            Assert.Equal(expectedUri, query.ToString());
            Assert.Equal(EmployeeType.PartTime, employee.EmpType);
            Assert.Equal("Employee Two", employee.Name);
        }

        [Fact]
        public void UseWhereToFilterByEnumKey_WithEnumAsKey_DoNotThrowException()
        {
            // Arrange
            string expectedUri = $"{ServiceRoot}/Employees?$filter=EmpType eq Microsoft.OData.Client.Tests.Tracking.EmployeeType'PartTime'";

            string response = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#Employees"",
    ""value"": [
        {
            ""EmpNumber"": 8,
            ""EmpType"": ""PartTime"",
            ""OrgId"": 1,
            ""Name"": ""Employee 45""
        }
    ]
}";
            SetupContextWithRequestPipeline(_defaultContext, response, "Employees");
            _defaultContext.SendingRequest2 += (sender, args) =>
            {
                Assert.Equal($"{expectedUri}&$top=1", args.RequestMessage.Url.ToString());
            };

            // Act
            var query = _defaultContext.Employees.Where(e => e.EmpType == EmployeeType.PartTime);
            Employee employee = query.First();

            // Assert
            Assert.Equal(expectedUri, query.ToString());
            Assert.Equal(8, employee.EmpNumber);
            Assert.Equal(EmployeeType.PartTime, employee.EmpType);
        }

        [Fact]
        public async Task FilterByCompositeKeys_WithEnumAsKey_DoNotThrowException()
        {
            // Arrange
            string expectedUri = $"{ServiceRoot}/Employees(EmpNumber=8,EmpType=Microsoft.OData.Client.Tests.Tracking.EmployeeType'PartTime',OrgId=1)";

            string response = @"{
    ""@odata.context"": ""http://localhost:8007/$metadata#Employees"",
    ""value"": [
        {
            ""EmpNumber"": 8,
            ""EmpType"": ""PartTime"",
            ""OrgId"": 1,
            ""Name"": ""Employee 24""
        }
    ]
}";
            SetupContextWithRequestPipeline(_defaultContext, response, "Employees");
            _defaultContext.SendingRequest2 += (sender, args) =>
            {
                Assert.Equal(expectedUri, args.RequestMessage.Url.ToString());
            };

            // Act
            EmployeeSingle query = _defaultContext.Employees.ByKey(
                new Dictionary<string, object>() { { "EmpNumber", 8 }, { "EmpType", EmployeeType.PartTime }, { "OrgId", 1 } });

            Employee employee = await query.GetValueAsync().ConfigureAwait(false);

            // Assert
            Assert.Equal(expectedUri, query.Query.ToString());
            Assert.Equal("Employee 24", employee.Name);
            Assert.Equal(EmployeeType.PartTime, employee.EmpType);
        }

        [Fact]
        public void FilterByEnumKey_WithEnumAsKey_DoNotThrowException()
        {
            // Arrange
            string expectedUri = $"{ServiceRoot}/Employees(Microsoft.OData.Client.Tests.Tracking.EmployeeType'FullTime')";

            string response = @"{
        ""@odata.context"": ""http://localhost:8007/$metadata#Employees"",
        ""value"": [
        {
            ""EmpNumber"": 9,
            ""EmpType"": ""FullTime"",
            ""OrgId"": 1,
            ""Name"": ""John Doe""
        }
    ]
}";
            SetupContextWithRequestPipeline(_defaultContext, response, "Employees");
            _defaultContext.SendingRequest2 += (sender, args) =>
            {
                Assert.Equal(expectedUri, args.RequestMessage.Url.ToString());
            };

            // Act
            EmployeeSingle query = _defaultContext.Employees.ByKey(
                new Dictionary<string, object>() { { "EmpType", EmployeeType.FullTime } });

            Employee employee = query.GetValue();

            // Assert
            Assert.Equal(expectedUri, query.Query.ToString());
            Assert.Equal(9, employee.EmpNumber);
            Assert.Equal(EmployeeType.FullTime, employee.EmpType);
        }

        private void SetupContextWithRequestPipeline(DataServiceContext context, string response, string path)
        {
            string location = $"{ServiceRoot}/{path}";

            context.Configurations.RequestPipeline.OnMessageCreating = (args) => new CustomizedRequestMessage(
                args,
                response,
                new Dictionary<string, string>()
                {
                    { "Content-Type", "application/json;charset=utf-8" },
                    { "Location", location },
                });
        }

        class Container : DataServiceContext
        {
            public Container(Uri serviceRoot) :
                base(serviceRoot, ODataProtocolVersion.V4)
            {
                Format.LoadServiceModel = () => CsdlReader.Parse(XmlReader.Create(new StringReader(Edmx)));
                Format.UseJson();
                Employees = base.CreateQuery<Employee>("Employees");
            }

            public DataServiceQuery<Employee> Employees { get; private set; }
        }
    }

    [Key("EmpNumber", "EmpType", "OrgId")]
    public class Employee : BaseEntityType, INotifyPropertyChanged
    {
        public int EmpNumber { get; set; }

        // Enum - Employee Type Key
        public EmployeeType EmpType { get; set; }

        public int OrgId { get; set; }

        public string Name { get; set; }

        [ForeignKey("OrgId")]
        public virtual Organization Organization { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public enum EmployeeType
    {
        None = 1,
        FullTime = 2,
        PartTime = 3
    }

    public partial class EmployeeSingle : DataServiceQuerySingle<Employee>
    {
        /// <summary>
        /// Initialize a new EmployeeSingle object.
        /// </summary>
        public EmployeeSingle(DataServiceContext context, string path)
            : base(context, path) { }
    }

    public static class ExtensionMethods
    {
        public static EmployeeSingle ByKey(this DataServiceQuery<Employee> _source, IDictionary<string, object> _keys)
        {
            return new EmployeeSingle(_source.Context, _source.GetKeyPath(Serializer.GetKeyString(_source.Context, _keys)));
        }
    }
}
