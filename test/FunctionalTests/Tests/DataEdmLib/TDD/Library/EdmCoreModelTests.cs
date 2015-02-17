//---------------------------------------------------------------------
// <copyright file="EdmCoreModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Edm.TDD.Tests
{
    using System.Collections.ObjectModel;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EdmCoreModelTests
    {
        [TestMethod]
        public void FindOperationByBindingParameterTypeShouldReturnNull()
        {
            EdmCoreModel.Instance.FindDeclaredBoundOperations(null).Should().BeEmpty();
        }
    }
}
