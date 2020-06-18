//---------------------------------------------------------------------
// <copyright file="EdmCoreModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmCoreModelTests
    {
        [Fact]
        public void FindOperationByBindingParameterTypeShouldReturnNull()
        {
            Assert.Empty(EdmCoreModel.Instance.FindDeclaredBoundOperations(null));
        }

        [Fact]
        public void NamespacePropertyShouldReturnCorrectNamespaceString()
        {
            Assert.Equal("Edm", EdmCoreModel.Namespace);
        }

        [Fact]
        public void SchemaElementsShouldReturnCorrectSchemaElementCount()
        {
            Assert.Equal(42, EdmCoreModel.Instance.SchemaElements.Count());
        }

        [Theory]
        [InlineData("Edm.Int32")]
        [InlineData("Int32")]
        [InlineData("Edm.Double")]
        [InlineData("Double")]
        [InlineData("Edm.Geography")]
        [InlineData("Geography")]
        [InlineData("Edm.EntityType")]
        [InlineData("EntityType")]
        [InlineData("Edm.ComplexType")]
        [InlineData("ComplexType")]
        [InlineData("Edm.Untyped")]
        [InlineData("Untyped")]
        [InlineData("Edm.AnnotationPath")]
        [InlineData("AnnotationPath")]
        [InlineData("Edm.PropertyPath")]
        [InlineData("PropertyPath")]
        [InlineData("Edm.NavigationPropertyPath")]
        [InlineData("NavigationPropertyPath")]
        public void FindDeclaredTypeShouldReturnCorrectSchemaType(string qualifiedName)
        {
            IEdmSchemaType edmType = EdmCoreModel.Instance.FindDeclaredType(qualifiedName);

            Assert.NotNull(edmType);
        }

        [Theory]
        [InlineData("Edm.Unknown")]
        [InlineData("Unknown")]
        [InlineData("NS.EntityType")]
        public void FindDeclaredTypeShouldReturnNullForUnknownTypeName(string qualifiedName)
        {
            IEdmSchemaType edmType = EdmCoreModel.Instance.FindDeclaredType(qualifiedName);

            Assert.Null(edmType);
        }

        [Theory]
        [InlineData("Edm.AnnotationPath", EdmPathTypeKind.AnnotationPath)]
        [InlineData("Edm.PropertyPath", EdmPathTypeKind.PropertyPath)]
        [InlineData("Edm.NavigationPropertyPath", EdmPathTypeKind.NavigationPropertyPath)]
        public void GetPathTypeKindShouldReturnCorrectPathTypeKind(string qualifiedName, EdmPathTypeKind kind)
        {
            Assert.Equal(kind, EdmCoreModel.Instance.GetPathTypeKind(qualifiedName));
        }
    }
}
