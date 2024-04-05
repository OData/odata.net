//---------------------------------------------------------------------
// <copyright file="TypeUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Tests
{
    public class TypeUtilsTests
    {
        [Theory]
        [InlineData("Ns.TypeName", false)]
        [InlineData("Collection(Ns.TypeName)", true)]
        public void ParseQualifiedTypeNameSplitsInputIntoNamespaceAndUnqualifiedTypeName(string input, bool isCollection)
        {
            // Act
            TypeUtils.ParseQualifiedTypeName(input, out string namespaceName, out string typeName, out bool outIsCollection);

            // Assert
            Assert.Equal("Ns", namespaceName);
            Assert.Equal("TypeName", typeName);
            Assert.Equal(isCollection, outIsCollection);
        }

        [Theory]
        [InlineData("TypeName")]
        [InlineData("Collection(TypeName)")]
        public void ParseQualifiedTypeNameThrowsExceptionForTypeNameNotQualified(string input)
        {
            // Arrange & Act
            var exception = Assert.Throws<ODataException>(() => TypeUtils.ParseQualifiedTypeName(input, out _, out _, out _));

            // Assert
            Assert.Equal("The value 'TypeName' is not a qualified type name. A qualified type name is expected.", exception.Message);
        }
    }
}
