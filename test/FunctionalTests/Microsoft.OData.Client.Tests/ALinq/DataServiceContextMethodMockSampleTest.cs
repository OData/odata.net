//---------------------------------------------------------------------
// <copyright file="DataServiceContextMethodMockSampleTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Moq;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    /// <summary>
    /// How to mock the DataServiceContext class methods using moq framework example.
    /// </summary>

    public class DataServiceContextMethodMockSampleTest
    {
        [Fact]
        public void VerifyIfDataObjectMethodIsMockable()
        {
            string str = "user";
            Mock<DataServiceContext> mockObject = new Mock<DataServiceContext>();
            mockObject.Setup(x => x.AddObject(It.IsAny<string>(), It.IsAny<object>())).Callback(()=> { str = "Saved Successfully"; });
            SaveUserDetails saveUserDetails = new SaveUserDetails() ;
            string result = saveUserDetails.SaveUser(mockObject.Object);
            Assert.Equal(str, result);
        }
    }

    /// <summary>
    /// A sample implementation class with methods calling the DataserviceContext.cs methods
    /// </summary>
    public class SaveUserDetails
    {
        /// <summary>
        /// A simple method to save a user's details
        /// </summary>
        /// <param name="DataServiceContext">
        /// A concrete class in the Microsoft.OData.Client library with implemented methods that this method calls.        
        /// </param>
        /// <returns>
        /// return "Saved Successfully if the AddObject method runs successfully".
        /// </returns>
        public string SaveUser(DataServiceContext dataServiceContext)
        {
            string str = "user";
            object obj = null;
            dataServiceContext.AddObject(str,obj);
            return "Saved Successfully";
        }
  
    }

}

