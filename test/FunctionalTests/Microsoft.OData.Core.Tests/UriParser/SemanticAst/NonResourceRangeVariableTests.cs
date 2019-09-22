//---------------------------------------------------------------------
// <copyright file="NonentityRangeVariableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the NonResourceRangeVariable class
    /// </summary>
    public class NonResourceRangeVariableTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new NonResourceRangeVariable(null, EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            Assert.Throws<ArgumentNullException>("name", createWithNullName);
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            NonResourceRangeVariable nonentityRangeVariable = new NonResourceRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            Assert.Equal("stuff", nonentityRangeVariable.Name);
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            var typeReference = EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true);
            NonResourceRangeVariable nonentityRangeVariable = new NonResourceRangeVariable("stuff", typeReference, null);
            Assert.Same(typeReference, nonentityRangeVariable.TypeReference);
        }

        [Fact]
        public void DissallowEntityType()
        {
            var entityTypeRef = HardCodedTestModel.GetPersonTypeReference();
            Action ctor = () => new NonResourceRangeVariable("abc", entityTypeRef, null);
            ctor.Throws<ArgumentException>(Strings.Nodes_NonentityParameterQueryNodeWithEntityType(entityTypeRef.FullName()));
        }

        [Fact]
        public void KindIsNonEntityRangeVariable()
        {
            NonResourceRangeVariable nonentityRangeVariable = new NonResourceRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            Assert.Equal(RangeVariableKind.NonResource, nonentityRangeVariable.Kind);
        }
    }
}
