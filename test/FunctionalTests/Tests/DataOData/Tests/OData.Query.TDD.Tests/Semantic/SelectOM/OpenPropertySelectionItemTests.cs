//---------------------------------------------------------------------
// <copyright file="OpenPropertySelectionItemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.SelectOM
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OpenPropertySelectionItemTests
    {
        [TestMethod]
        public void ConstructorShouldSetPropertyName()
        {
            var item = new PathSelectItem(new ODataSelectPath(new OpenPropertySegment("abc")));
            item.SelectedPath.FirstSegment.ShouldBeOpenPropertySegment("abc");
        }

        [TestMethod]
        public void PathCannotBeNull()
        {
            Action ctor = () => new PathSelectItem(null);
            ctor.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void NullNameIsNotAllowed()
        {
            Action ctor = () => new PathSelectItem(null);
            ctor.ShouldThrow<ArgumentNullException>();
        }
    }
}
