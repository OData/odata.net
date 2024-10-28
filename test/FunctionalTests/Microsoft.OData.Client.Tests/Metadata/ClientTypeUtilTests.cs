//---------------------------------------------------------------------
// <copyright file="ClientTypeUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client.Metadata;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Microsoft.OData.Client.Tests.Metadata
{
    /// <summary>
    /// A class test to test whether the use of Key Attribute on a Type property can identify a type as an entity when the conventional key naming methods are not used.
    /// </summary>
    public class ClientTypeUtilTests
    {
        [Theory]
        [InlineData(typeof(Giraffe), true)]
        [InlineData(typeof(Hippo), true)]
        [InlineData(typeof(Ferret), true)]
        [InlineData(typeof(Lion), false)]
        public void IfTypeProperty_HasConventionalKey_TypeIsEntity(Type entityType, bool isEntity)
        {
            //Act
            bool actualResult = ClientTypeUtil.TypeOrElementTypeIsEntity(entityType);
            //Assert
            Assert.Equal(actualResult, isEntity);
        }

        [Fact]
        public void IFTypeProperty_HasKeyAttribute_TypeIsEntity()
        {
            //Arrange
            Type person = typeof(Person);
            //Act
            bool actualResult = ClientTypeUtil.TypeOrElementTypeIsEntity(person);
            //Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void IFTypeProperty_HasNoKeyAttribute_TypeIsNotEntity()
        {
            //Arrange
            Type student = typeof(Student);
            //Act
            bool actualResult = ClientTypeUtil.TypeOrElementTypeIsEntity(student);
            //Assert
            Assert.False(actualResult);
        }

        [Fact]
        public void IFTypeProperty_HasKeyAttributeAndOneProperty_TypeIsEntityAndDoesNotThrowException()
        {
            //Arrange
            Type car = typeof(Car);

            //Act
            bool actualResult = ClientTypeUtil.TypeOrElementTypeIsEntity(car);

            //Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void IFType_HasMultipleKeyAttributesWhereOneIsEnum_TypeIsEntityAndDoesNotThrowException()
        {
            //Arrange
            Type employee = typeof(Employee);

            //Act
            bool actualResult = ClientTypeUtil.TypeOrElementTypeIsEntity(employee);

            //Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void IFTypeProperty_HasMultipleKeyAttributes_GetKeyPropertiesOnType_DoesNotThrowException()
        {
            //Arrange
            Type employee = typeof(Employee);

            int expectedNumberOfKeyProperties = 4; // 2 Primitive Known Types, 1 Enum Type, 1 Enum Nullable Generic Type

            //Act
            PropertyInfo[] keyProperties = ClientTypeUtil.GetKeyPropertiesOnType(employee);

            //Assert
            Assert.Equal(expectedNumberOfKeyProperties, keyProperties.Length);
        }

        [Fact]
        public void IFTypeProperty_HasEnumTypeKeyAttribute_GetKeyPropertiesOnType_DoesNotThrowException()
        {
            // Arrange
            Type employee = typeof(Employee);

            //Act
            PropertyInfo[] keyProperties = ClientTypeUtil.GetKeyPropertiesOnType(employee);
            PropertyInfo key = keyProperties.Single(k => k.Name == "EmpType");

            //Assert
            Assert.True(key.PropertyType.IsEnum());
            Assert.True(key.PropertyType == typeof(EmployeeType));
        }

        [Fact]
        public void IFTypeProperty_HasKnownPrimitiveTypesKeyAttributes_GetKeyPropertiesOnType_DoesNotThrowException()
        {
            // Arrange
            Type employee = typeof(Employee);

            //Act
            PropertyInfo[] keyProperties = ClientTypeUtil.GetKeyPropertiesOnType(employee);

            PropertyInfo empNumKey = keyProperties.Single(k => k.Name == "EmpNumber");
            PropertyInfo deptNumKey = keyProperties.Single(k => k.Name == "DeptNumber");

            //Assert
            Assert.True(PrimitiveType.IsKnownType(empNumKey.PropertyType) && empNumKey.PropertyType == typeof(int));
            Assert.True(PrimitiveType.IsKnownType(deptNumKey.PropertyType) && deptNumKey.PropertyType == typeof(string));
        }

        [Fact]
        public void IFTypeProperty_HasNullableGenericTypeKeyAttribute_OfTypeEnum_GetKeyPropertiesOnType_DoesNotThrowException()
        {
            // Arrange
            Type employee = typeof(Employee);

            //Act
            PropertyInfo[] keyProperties = ClientTypeUtil.GetKeyPropertiesOnType(employee);
            PropertyInfo key = keyProperties.Single(k => k.Name == "NullableEmpType");

            //Assert
            Assert.True(key.PropertyType.IsGenericType);
            Assert.True(key.PropertyType == typeof(System.Nullable<EmployeeType>));
        }

        [Fact]
        public void IFTypeProperty_HasNullableGenericTypeKey_OfTypeStruct_GetKeyPropertiesOnType_ThrowsException()
        {
            // Arrange
            Type employee = typeof(EmployeeWithNullableStruct);

            PropertyInfo empTypeStructKey = employee.GetProperty("EmpTypeStruct");

            InvalidOperationException expectedException = Error.InvalidOperation(Strings.ClientType_KeysMustBeSimpleTypes(empTypeStructKey.Name, employee.ToString(), empTypeStructKey.PropertyType.FullName));

            //Act
            InvalidOperationException actualException = Assert.Throws<InvalidOperationException>(() => ClientTypeUtil.GetKeyPropertiesOnType(employee));

            //Assert
            Assert.NotNull(actualException);
            Assert.Equal(expectedException.Message, actualException.Message);
        }

        public class Person
        {
            [System.ComponentModel.DataAnnotations.Key]
            public int PId { get; set; }
            public string Name { get; set; }
        }

        public class Student
        {
            public int SId { get; set; }
            public string Name { get; set; }
        }

        public class Car
        {
            [System.ComponentModel.DataAnnotations.Key]
            public int NonStandardId { get; set; }
        }

        public class Employee
        {
            [System.ComponentModel.DataAnnotations.Key]
            public int EmpNumber { get; set; }

            [System.ComponentModel.DataAnnotations.Key]
            public string DeptNumber { get; set; }

            [System.ComponentModel.DataAnnotations.Key]
            public EmployeeType EmpType { get; set; }

            [System.ComponentModel.DataAnnotations.Key]
            public EmployeeType? NullableEmpType { get; set; }

            public string Name { get; set; }

            [System.ComponentModel.DataAnnotations.Schema.ForeignKey("DeptNumber")]
            public Department Department { get; set; }
        }

        public class EmployeeWithNullableStruct
        {
            [System.ComponentModel.DataAnnotations.Key]
            public int EmpNumber { get; set; }

            [System.ComponentModel.DataAnnotations.Key]
            public EmployeeTypeStruct? EmpTypeStruct { get; set; }

            public string Name { get; set; }
        }

        public enum EmployeeType
        {
            None = 1,
            FullTime = 2,
            PartTime = 3
        }

        public class Department
        {
            [System.ComponentModel.DataAnnotations.Key]
            public string DeptId { get; set; }
            public string Name { get; set; }
        }

        public struct EmployeeTypeStruct
        {
            public int EmpTypeId { get; set; }
        }

        public class Giraffe
        {
            /// <summary>
            /// Conventional Id Key property
            /// </summary>
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Hippo
        {
            /// <summary>
            /// Conventional {TypeName + Id} Key Property
            /// </summary>
            public int HippoId { get; set; }
            public double Weight { get; set; }
        }

        public class Ferret
        {
            /// <summary>
            /// Conventional {TypeName + Id} Key Property with odd casing
            /// </summary>
            public int FeRReTID { get; set; }
            public double Weight { get; set; }
        }

        public class Lion
        {
            public int SomeId { get; set; }
            public string Name { get; set; }
        }
    }
}
