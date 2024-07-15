//---------------------------------------------------------------------
// <copyright file="DataServiceContextQueryTests.cs" company="Microsoft">
// Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using System;
using System.Collections.Generic;
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
            SetupContextWithRequestPipeline(new DataServiceContext[] { _defaultContext }, response, "employees");

            // Act
            IEnumerable<Employee> employees = await _defaultContext.Employees.ExecuteAsync();

            // Assert
            Assert.Equal(2, employees.Count());
        }

        [Fact]
        public void SelectSpecificEntity_WithEnumAsKey_DoNotThrowException()
        {
            // Arrange
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
            SetupContextWithRequestPipeline(new DataServiceContext[] { _defaultContext }, response, "employees");

            // Act
            Employee employee = _defaultContext.Employees.Where(e => e.EmpNumber == 8).First();

            // Assert
            Assert.Equal(EmployeeType.PartTime, employee.EmpType);
            Assert.Equal("Employee Two", employee.Name);
        }

        private void SetupContextWithRequestPipeline(DataServiceContext[] contexts, string response, string path)
        {
            string location = $"{ServiceRoot}/{path}";

            foreach (var context in contexts)
            {
                context.Configurations.RequestPipeline.OnMessageCreating =
                    (args) => new CustomizedRequestMessage(
                        args,
                        response,
                        new Dictionary<string, string>()
                        {
                            { "Content-Type", "application/json;charset=utf-8" },
                            { "Location", location },
                        });
            }
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
    public class Employee : BaseEntityType
    {
        public int EmpNumber { get; set; }

        // Enum - Employee Type Key
        public EmployeeType EmpType { get; set; }

        public int OrgId { get; set; }

        public string Name { get; set; }

        [ForeignKey("OrgId")]
        public virtual Organization Organization { get; set; }
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
}
