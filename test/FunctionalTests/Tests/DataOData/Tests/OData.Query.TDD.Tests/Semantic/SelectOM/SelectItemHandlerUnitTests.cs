//---------------------------------------------------------------------
// <copyright file="SelectItemHandlerUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.SelectOM
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SelectItemHandlerUnitTests
    {
        private class FakeHandler : SelectItemHandler
        {
        }
        
        [TestMethod]
        public void WildcardSelectItemIsNotImplemented()
        {
            FakeHandler handler = new FakeHandler();
            WildcardSelectItem item = new WildcardSelectItem();
            Action visitWildcard = () => item.HandleWith(handler);
            visitWildcard.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void PathSelectionItemIsNotImplemented()
        {
            FakeHandler handler = new FakeHandler();
            PathSelectItem item = new PathSelectItem(new ODataSelectPath(){new PropertySegment(ModelBuildingHelpers.BuildValidPrimitiveProperty())});
            Action visitPath = () => item.HandleWith(handler);
            visitPath.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void ContainerQualifiedWildcardSelectItemIsNotImplemented()
        {
            FakeHandler handler = new FakeHandler();
            ContainerQualifiedWildcardSelectItem item = new ContainerQualifiedWildcardSelectItem(ModelBuildingHelpers.BuildValidEntityContainer());
            Action visitContainerQualifiedWildcard = () => item.HandleWith(handler);
            visitContainerQualifiedWildcard.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void ExpandedNavigationPropertySelectItemIsNotImplemented()
        {
            FakeHandler handler = new FakeHandler();
            ExpandedNavigationSelectItem item = new ExpandedNavigationSelectItem(new ODataExpandPath(){new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), ModelBuildingHelpers.BuildValidEntitySet())}, null, null);
            Action visitExpandedNavigationSelectItem = () => item.HandleWith(handler);
            visitExpandedNavigationSelectItem.ShouldThrow<NotImplementedException>();
        }
    }
}
