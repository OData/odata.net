//---------------------------------------------------------------------
// <copyright file="ClientTypeUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client.Metadata;
using System;
using Xunit;

namespace Microsoft.OData.Client.Tests.Metadata
{
    /// <summary>
    /// A class test to test whether the use of Key Attribute on a Type property can identify a type as an entity when the conventional key naming methods are not used.
    /// </summary>
    public class ClientTypeUtilTests
    {
#if !PORTABLELIB
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
#endif
    }
}
