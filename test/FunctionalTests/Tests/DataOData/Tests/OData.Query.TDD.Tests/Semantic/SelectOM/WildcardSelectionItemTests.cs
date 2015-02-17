//---------------------------------------------------------------------
// <copyright file="WildcardSelectionItemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.SelectOM
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WildcardSelectionItemTests
    {
        [TestMethod]
        public void ConstructorDoesNotThrow()
        {
            Action ctor = () => new WildcardSelectItem();
            ctor.ShouldNotThrow();
        }

        [TestMethod]
        public void TwoInstanceAreNotReferenceEquals()
        {
            var item1 = new WildcardSelectItem();
            var item2 = new WildcardSelectItem();
            item1.Should().NotBeSameAs(item2);
        }
    }
}
