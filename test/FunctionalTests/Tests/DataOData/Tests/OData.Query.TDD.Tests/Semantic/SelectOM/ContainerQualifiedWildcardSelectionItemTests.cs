//---------------------------------------------------------------------
// <copyright file="ContainerQualifiedWildcardSelectionItemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.SelectOM
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContainerQualifiedWildcardSelectionItemTests
    {
        [TestMethod]
        public void ContainerIsSetFromConstructor()
        {
            var item = new NamespaceQualifiedWildcardSelectItem("name.space");
            item.Namespace.Should().Be("name.space");
        }

        [TestMethod]
        public void ContainerCannotBeNull()
        {
            Action ctor = () => new NamespaceQualifiedWildcardSelectItem(null);
            ctor.ShouldThrow<ArgumentNullException>();
        }
    }
}
