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
            Assert.Equal(40, EdmCoreModel.Instance.SchemaElements.Count());
        }

        [Theory]
        [InlineData("Edm.Int32")]
        [InlineData("Int32")]
        [InlineData("int32")]
        [InlineData("Edm.Double")]
        [InlineData("Double")]
        [InlineData("double")]
        [InlineData("Edm.String")]
        [InlineData("String")]
        [InlineData("string")]
        [InlineData("Edm.Geography")]
        [InlineData("Geography")]
        [InlineData("geography")]
        [InlineData("Edm.EntityType")]
        [InlineData("EntityType")]
        [InlineData("entityType")]
        [InlineData("Edm.ComplexType")]
        [InlineData("ComplexType")]
        [InlineData("complextype")]
        [InlineData("Edm.Untyped")]
        [InlineData("Untyped")]
        [InlineData("untyped")]
        [InlineData("Edm.AnnotationPath")]
        [InlineData("AnnotationPath")]
        [InlineData("annotationPath")]
        [InlineData("Edm.PropertyPath")]
        [InlineData("PropertyPath")]
        [InlineData("propertyPath")]
        [InlineData("Edm.NavigationPropertyPath")]
        [InlineData("NavigationPropertyPath")]
        [InlineData("navigationpropertypath")]
        public void FindDeclaredTypeShouldReturnCorrectSchemaType(string qualifiedName)
        {
            IEdmSchemaType edmType = EdmCoreModel.Instance.FindDeclaredType(qualifiedName);

            Assert.NotNull(edmType);
        }

        [Theory]
        [InlineData("Edm.Unknown")]
        [InlineData("Unknown")]
		[InlineData("unknown")]
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetUntypedShouldReturnCorrectUntypedTypeReference(bool isNullable)
        {
            IEdmUntypedTypeReference untypedTypeReference = EdmCoreModel.Instance.GetUntyped(isNullable);
            Assert.NotNull(untypedTypeReference);
            Assert.Equal(isNullable, untypedTypeReference.IsNullable);

            IEdmUntypedType untypedType = EdmCoreModel.Instance.GetUntypedType();
            Assert.Same(untypedTypeReference.Definition, untypedType);
        }
    }
}
