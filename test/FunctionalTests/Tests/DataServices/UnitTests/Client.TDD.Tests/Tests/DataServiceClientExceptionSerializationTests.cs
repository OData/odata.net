//---------------------------------------------------------------------
// <copyright file="DataServiceClientExceptionSerializationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace AstoriaUnitTests.Tests.Client
{
    public class DataServiceClientExceptionSerializationTests
    {
        [Fact]
        public void When_DataServiceClientException_is_serialized_all_instance_data_is_saved()
        {
            // Arrange
            const string expectedMessage = "Custom message";
            const int expectedStatusCode = 705;

            var sut = new DataServiceClientException(expectedMessage, expectedStatusCode);

            var bf = new BinaryFormatter();
            var stream = new MemoryStream();

            // Act
            bf.Serialize(stream, sut);
            stream.Seek(0, SeekOrigin.Begin);
            var ds = (DataServiceClientException)bf.Deserialize(stream);

            // Assert
            Assert.Equal(expectedMessage, ds.Message);
            Assert.Equal(expectedStatusCode, ds.StatusCode);
        }
    }
}
