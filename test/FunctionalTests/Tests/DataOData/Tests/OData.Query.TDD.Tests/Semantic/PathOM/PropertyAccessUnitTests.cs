//---------------------------------------------------------------------
// <copyright file="PropertyAccessUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PropertyAccessUnitTests
    {
        [TestMethod]
        public void PropertyCannotBeNull()
        {
            Action createWithNullProperty = () => new PropertySegment(null);
            createWithNullProperty.ShouldThrow<Exception>(Error.ArgumentNull("property").ToString());
        }

        [TestMethod]
        public void PropertySetCorrectly()
        {
            IEdmStructuralProperty prop = ModelBuildingHelpers.BuildValidPrimitiveProperty();
            PropertySegment segment = new PropertySegment(prop);
            segment.Property.Should().Be(prop);
        }
    }
}
