//---------------------------------------------------------------------
// <copyright file="EdmCoreModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmCoreModelTests
    {
        [Fact]
        public void FindOperationByBindingParameterTypeShouldReturnNull()
        {
            EdmCoreModel.Instance.FindDeclaredBoundOperations(null).Should().BeEmpty();
        }
    }
}
