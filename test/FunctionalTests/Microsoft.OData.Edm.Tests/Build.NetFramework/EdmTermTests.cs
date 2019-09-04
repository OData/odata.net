//---------------------------------------------------------------------
// <copyright file="EdmEntityContainerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{

    public class EdmTermTests
    {
        [Fact]
        public void EdmModelAddTermPrimitiveTypeTest()
        {
            var model = new EdmModel();
            Assert.Equal(
                model.AddTerm("NS", "Term1", EdmPrimitiveTypeKind.String),
                model.FindTerm("NS.Term1"));
        }

        [Fact]
        public void EdmModelAddTermTypeReferenceTest()
        {
            var boolType = EdmCoreModel.Instance.GetBoolean(false);

            var model = new EdmModel();
            Assert.Equal(
                model.AddTerm("NS", "Term1", boolType),
                model.FindTerm("NS.Term1"));
        }
    }
}
