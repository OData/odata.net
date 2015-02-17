//---------------------------------------------------------------------
// <copyright file="DataServiceExceptionSerializationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.Service;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AstoriaUnitTests.Tests.Server
{
    [TestClass]
    public class DataServiceExceptionSerializationTests
    {
        [TestMethod]
        public void When_DataServiceException_is_serialized_all_public_instance_data_is_saved()
        {
            // Arrange
            const int expectedStatusCode = 705;
            const string expectedErrorCode = "A47";
            const string expectedMessage = "Server error.";
            const string expectedMessageLang = "en";

            var sut = new DataServiceException(
                expectedStatusCode, expectedErrorCode, expectedMessage, expectedMessageLang, innerException: null);

            // Act
            var ds = SerializeAndDeserializeDataServiceException(sut);

            // Assert
            Assert.AreNotSame(sut, ds);
            Assert.AreEqual(sut.GetType(), ds.GetType());
            Assert.AreEqual(expectedMessage, ds.Message);
            Assert.AreEqual(expectedStatusCode, ds.StatusCode);
            Assert.AreEqual(expectedErrorCode, ds.ErrorCode);
            Assert.AreEqual(expectedMessageLang, ds.MessageLanguage);
        }

        [TestMethod]
        public void When_DataServiceException_is_serialized_all_internal_instance_data_is_saved()
        {
            // Arrange
            const string expectedResponseAllowHeader = "None";

            var sut = DataServiceException.CreateMethodNotAllowed("Message", expectedResponseAllowHeader);

            var ds = SerializeAndDeserializeDataServiceException(sut);

            // Assert
            Assert.AreEqual(expectedResponseAllowHeader, ds.ResponseAllowHeader);
        }

        private static DataServiceException SerializeAndDeserializeDataServiceException(DataServiceException sut)
        {
            var bf = new BinaryFormatter();
            var stream = new MemoryStream();

            bf.Serialize(stream, sut);
            stream.Seek(0, SeekOrigin.Begin);
            return (DataServiceException)bf.Deserialize(stream);
        }
    }
}
